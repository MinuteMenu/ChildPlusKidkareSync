using ChildPlusKidkareSync.Core.Constants;
using ChildPlusKidkareSync.Core.Enums;
using ChildPlusKidkareSync.Core.Models.ChildPlus;
using ChildPlusKidkareSync.Core.Models.Configuration;
using ChildPlusKidkareSync.Core.Models.Kidkare;
using ChildPlusKidkareSync.Core.Models.Sync;
using ChildPlusKidkareSync.Infrastructure.Data;
using ChildPlusKidkareSync.Infrastructure.Mapping;
using ChildPlusKikareSync.Core.Models.ChildPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Data;

namespace ChildPlusKidkareSync.Infrastructure.Services;

public interface ISyncService
{
    Task<List<SyncResult>> SyncAllTenantsAsync(List<TenantConfiguration> tenants, SyncConfiguration config);
}

public class SyncService : ISyncService
{
    private readonly ILogger<SyncService> _logger;
    private readonly IChildPlusRepository _childPlusRepository;
    private readonly ISyncLogRepository _syncLogRepository;
    private readonly IDataMapper _dataMapper;
    private readonly IServiceProvider _serviceProvider;

    public SyncService(
        ILogger<SyncService> logger,
        IChildPlusRepository childPlusRepository,
        ISyncLogRepository syncLogRepository,
        IDataMapper dataMapper,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _childPlusRepository = childPlusRepository;
        _syncLogRepository = syncLogRepository;
        _dataMapper = dataMapper;
        _serviceProvider = serviceProvider;
    }

    #region Public Methods

    /// <summary>
    /// Sync all tenants in parallel
    /// </summary>
    public async Task<List<SyncResult>> SyncAllTenantsAsync(
        List<TenantConfiguration> tenants,
        SyncConfiguration config)
    {
        _logger.LogInformation("Starting sync for {Count} tenants", tenants.Count);
        var results = new ConcurrentBag<SyncResult>();

        var enabledTenants = tenants.Where(t => t.Enabled).ToList();

        // Process tenants in parallel
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = config.MaxParallelTenants
        };

        await Parallel.ForEachAsync(enabledTenants, parallelOptions, async (tenant, ct) =>
        {
            try
            {
                var result = await SyncSingleTenantAsync(tenant, config);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync tenant {TenantId}", tenant.TenantId);
                results.Add(new SyncResult
                {
                    RequestId = Guid.NewGuid(),
                    TenantId = tenant.TenantId,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow,
                    FailedCount = 1,
                    Errors = new List<string> { ex.Message }
                });
            }
        });

        _logger.LogInformation("Completed sync for all tenants. Total: {Count}, Success: {Success}, Failed: {Failed}",
            results.Count,
            results.Count(r => r.IsSuccess),
            results.Count(r => !r.IsSuccess));

        return results.ToList();
    }

    #endregion

    #region Private Methods - Tenant Level

    /// <summary>
    /// Sync a single tenant: Sites, Staff, Children
    /// </summary>
    private async Task<SyncResult> SyncSingleTenantAsync(
        TenantConfiguration tenant,
        SyncConfiguration config)
    {
        var requestId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;

        _logger.LogInformation("Starting sync for tenant {TenantId} ({TenantName}) - RequestId: {RequestId}", tenant.TenantId, tenant.TenantName, requestId);

        var result = new SyncResult
        {
            RequestId = requestId,
            TenantId = tenant.TenantId,
            StartTime = startTime
        };

        try
        {
            // Create KidkareService for this tenant
            var kidkareService = CreateKidkareServiceForTenant(tenant);

            // Step 1: Get all sites
            var sites = await _childPlusRepository.GetSitesAsync(tenant.TenantId, tenant.ChildPlusConnectionString);
            _logger.LogInformation("Found {Count} sites for tenant {TenantId}", sites.Count, tenant.TenantId);

            if (!sites.Any())
            {
                _logger.LogWarning("No sites found for tenant {TenantId}", tenant.TenantId);
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            // Step 2: Sync Sites (Centers) and Staff in parallel
            var siteResults = await SyncSitesAndStaffAsync(tenant, sites, config, requestId, kidkareService);

            // Step 3: Sync all Children in batches
            var childrenResult = await SyncAllChildrenInBatchesAsync(tenant, sites, config.BatchSize, requestId, kidkareService);

            // Aggregate results
            result.SuccessCount = siteResults.success + childrenResult.success;
            result.FailedCount = siteResults.failed + childrenResult.failed;
            result.SkippedCount = siteResults.skipped + childrenResult.skipped;
            result.TotalRecords = result.SuccessCount + result.FailedCount + result.SkippedCount;
            result.EndTime = DateTime.UtcNow;

            var duration = (result.EndTime - result.StartTime).TotalSeconds;
            _logger.LogInformation("Completed sync for tenant {TenantId} in {Duration}s - Success: {Success}, Failed: {Failed}, Skipped: {Skipped}",
                tenant.TenantId, duration, result.SuccessCount, result.FailedCount, result.SkippedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing tenant {TenantId}", tenant.TenantId);
            result.FailedCount++;
            result.Errors.Add($"Tenant sync failed: {ex.Message}");
            result.EndTime = DateTime.UtcNow;
            return result;
        }
    }

    /// <summary>
    /// Create KidkareService instance for specific tenant with its own API key
    /// </summary>
    private IKidkareService CreateKidkareServiceForTenant(TenantConfiguration tenant)
    {
        var clientLogger = _serviceProvider.GetRequiredService<ILogger<KidkareClient>>();
        var serviceLogger = _serviceProvider.GetRequiredService<ILogger<KidkareService>>();

        var client = new KidkareClient(
            tenant.KidkareApiBaseUrl,
            tenant.KidkareApiKey,
            clientLogger);

        return new KidkareService(client, serviceLogger);
    }

    #endregion

    #region Private Methods - Sites & Staff Level

    /// <summary>
    /// Sync all sites (centers) and their staff in parallel
    /// </summary>
    private async Task<(int success, int failed, int skipped)> SyncSitesAndStaffAsync(
        TenantConfiguration tenant,
        List<ChildPlusSite> sites,
        SyncConfiguration config,
        Guid requestId,
        IKidkareService kidkareService)
    {
        var results = new ConcurrentBag<(int success, int failed, int skipped)>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = config.MaxParallelSites
        };

        await Parallel.ForEachAsync(sites, parallelOptions, async (site, ct) =>
        {
            var siteResult = await SyncSingleSiteWithStaffAsync(tenant, site, requestId, kidkareService);
            results.Add(siteResult);
        });

        // Aggregate
        var totalSuccess = results.Sum(r => r.success);
        var totalFailed = results.Sum(r => r.failed);
        var totalSkipped = results.Sum(r => r.skipped);

        return (totalSuccess, totalFailed, totalSkipped);
    }

    /// <summary>
    /// Sync one site (center) and all its staff
    /// </summary>
    private async Task<(int success, int failed, int skipped)> SyncSingleSiteWithStaffAsync(
        TenantConfiguration tenant,
        ChildPlusSite site,
        Guid requestId,
        IKidkareService kidkareService)
    {
        int success = 0, failed = 0, skipped = 0;

        try
        {
            // 1. Sync Center
            var centerResult = await SyncCenterAsync(tenant, site, requestId, kidkareService);

            if (centerResult.Action == SyncAction.Update || centerResult.Action == SyncAction.Insert)
                success++;
            else if (centerResult.Action == SyncAction.Skip)
                skipped++;
            else
                failed++;

            // 2. If new center was created, setup roles and permissions
            if (centerResult.Action == SyncAction.Insert && centerResult.CenterResponse != null)
            {
                await CreateDefaultRolesAndPermissionsAsync(tenant, centerResult.CenterResponse.CenterId, site.CenterId, requestId, kidkareService);
            }

            // 3. Sync Staff for this center
            var staffList = await _childPlusRepository.GetStaffByCenterIdAsync(tenant.ChildPlusConnectionString, site.CenterId);
            if (!staffList.Any()) return (success, failed, skipped);

            var rolesListResponse = await kidkareService.GetRoleAsync(centerResult.CenterResponse.CenterId);
            if (rolesListResponse?.IsSuccess != true || rolesListResponse.Data == null)
            {
                _logger.LogWarning("No roles retrieved for Center ID {CenterId}", centerResult.CenterResponse.CenterId);
                return (success, failed, skipped);
            }

            // Roles abbreviations
            var roles = new Dictionary<string, string>
                    {
                        { "Teacher", "-T" },
                        { "Owner/Director", "-O" },
                        { "Admin", "-A" }
                    };

            const int maxConcurrency = 8;
            using var sem = new SemaphoreSlim(maxConcurrency);

            var rolesList = rolesListResponse.Data;

            var tasks = new List<Task>();
            foreach (var staff in staffList)
            {
                // For each staff, attempt all role variants (Teacher/Owner/Admin)
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var (roleName, roleAbbr) in roles)
                    {
                        await sem.WaitAsync();
                        try
                        {
                            var roleInfo = rolesList.FirstOrDefault(r => r.RoleName == roleName);
                            if (roleInfo == null)
                            {
                                _logger.LogWarning("Role '{RoleName}' not found for Center ID {CenterId}", roleName, centerResult.CenterResponse.CenterId);
                                continue;
                            }

                            var staffRequest = _dataMapper.MapToKidkareStaff(centerResult.CenterResponse.CenterId, roleAbbr, roleInfo, staff);
                            var staffAction = await SyncStaffAsync(tenant, staff, requestId, staffRequest, kidkareService);

                            if (staffAction == SyncAction.Update || staffAction == SyncAction.Insert)
                                success++;
                            else if (staffAction == SyncAction.Skip)
                                skipped++;
                            else
                                failed++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error syncing staff {StaffId} for center {CenterId}", staff.StaffId, centerResult.CenterResponse.CenterId);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return (success, failed, skipped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing site {CenterId}", site.CenterId);
            return (success, failed + 1, skipped);
        }
    }

    /// <summary>
    /// Sync a single center/site
    /// </summary>
    private async Task<CenterSyncResult> SyncCenterAsync(
        TenantConfiguration tenant,
        ChildPlusSite site,
        Guid requestId,
        IKidkareService kidkareService)
    {
        try
        {
            // Check duplicate
            var shouldSync = await _syncLogRepository.ShouldSyncAsync(
                tenant.KidkareCxSqlConnectionString,
                EntityType.Center.ToString(),
                site.CenterId,
                site.Timestamp);

            if (!shouldSync)
            {
                await LogSyncAsync(tenant, EntityType.Center, site.CenterId, null,
                    SyncAction.Skip, SyncStatus.Success, "No changes detected", site.Timestamp, site.CenterId, requestId);

                return new CenterSyncResult
                {
                    Action = SyncAction.Skip,
                    CenterResponse = null
                };
            }

            // Determine if this is Insert or Update based on previous sync log
            var lastLog = await _syncLogRepository.GetLastSyncLogAsync(tenant.KidkareCxSqlConnectionString, EntityType.Center.ToString(), site.CenterId);

            bool isNewCenter = lastLog == null;

            // Map and call API
            var centerRequest = _dataMapper.MapToKidkareCenter(site);
            var response = await kidkareService.SaveCenterAsync(centerRequest);

            if (response.IsSuccess && response.Data != null)
            {
                var action = isNewCenter ? SyncAction.Insert : SyncAction.Update;

                // Parse Kidkare response to get CenterResponse
                var centerResponse = ParseCenterResponse(response.Data);

                if (centerResponse == null)
                {
                    _logger.LogWarning("Failed to parse center response for {CenterId}", site.CenterId);

                    await LogSyncAsync(tenant, EntityType.Center, site.CenterId, null,
                        SyncAction.Error, SyncStatus.Failed, "Failed to parse center response",
                        site.Timestamp, site.CenterId, requestId);

                    return new CenterSyncResult
                    {
                        Action = SyncAction.Error,
                        CenterResponse = null
                    };
                }

                // Log center sync
                await LogSyncAsync(tenant, EntityType.Center, site.CenterId,
                    centerResponse.CenterId.ToString(),
                    action, SyncStatus.Success, response.Message,
                    site.Timestamp, site.CenterId, requestId);

                return new CenterSyncResult
                {
                    Action = action,
                    CenterResponse = centerResponse
                };
            }
            else
            {
                await LogSyncAsync(tenant, EntityType.Center, site.CenterId, null,
                    SyncAction.Error, SyncStatus.Failed, response.Message,
                    site.Timestamp, site.CenterId, requestId);

                return new CenterSyncResult
                {
                    Action = SyncAction.Error,
                    CenterResponse = null
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing center {CenterId}", site.CenterId);
            await LogSyncAsync(tenant, EntityType.Center, site.CenterId, null,
                SyncAction.Error, SyncStatus.Failed, ex.Message,
                site.Timestamp, site.CenterId, requestId);

            return new CenterSyncResult
            {
                Action = SyncAction.Error,
                CenterResponse = null
            };
        }
    }

    /// <summary>
    /// Parse center response from Kidkare API
    /// </summary>
    private CenterResponse ParseCenterResponse(object responseData)
    {
        try
        {
            var jObj = Newtonsoft.Json.Linq.JObject.FromObject(responseData);
            var centerInfo = jObj["data"]?["CenterInfo"];

            if (centerInfo != null)
            {
                return centerInfo.ToObject<CenterResponse>();
            }

            // Fallback: try direct deserialization
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CenterResponse>(responseData.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing center response");
            return null;
        }
    }

    /// <summary>
    /// Create default roles and permissions for a new center with concurrency control
    /// </summary>
    private async Task CreateDefaultRolesAndPermissionsAsync(
        TenantConfiguration tenant,
        int centerId,           // Kidkare's CenterId (int)
        string sourceCenterId,  // ChildPlus's CenterId (string)
        Guid requestId,
        IKidkareService kidkareService)
    {
        _logger.LogInformation("Creating default roles and permissions for new center {CenterId}", centerId);

        try
        {
            // Get role-permission mappings
            var rolePermissions = RolePermissionsFactory.InitializeRolesAndPermissionsForCenter(centerId);

            // Process each role SEQUENTIALLY (must wait for role creation before permissions)
            foreach (var (roleName, permissions) in rolePermissions)
            {
                try
                {
                    // ═══════════════════════════════════════════════════════
                    // STEP 1: CREATE ROLE
                    // ═══════════════════════════════════════════════════════
                    var rolePayload = new RoleModel
                    {
                        RoleName = roleName,
                        CenterId = centerId
                    };

                    var assignRoleResp = await kidkareService.AssignRoleAsync(rolePayload);

                    if (assignRoleResp?.IsSuccess == true && assignRoleResp.Data != null)
                    {
                        _logger.LogInformation("Role '{RoleName}' assigned for center {CenterId}, RoleId={RoleId}", roleName, centerId, assignRoleResp.Data.RoleId);

                        // Log role creation to SyncLogTable
                        await LogSyncAsync(tenant, EntityType.Center,
                            $"{sourceCenterId}_Role_{roleName}",
                            assignRoleResp.Data.RoleId.ToString(),
                            SyncAction.Insert, SyncStatus.Success,
                            $"Role created: {roleName}",
                            null, sourceCenterId, requestId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to assign role '{RoleName}' for center {CenterId}: {Message}", roleName, centerId, assignRoleResp?.Message);

                        await LogSyncAsync(tenant, EntityType.Center,
                            $"{sourceCenterId}_Role_{roleName}",
                            null,
                            SyncAction.Error, SyncStatus.Failed,
                            assignRoleResp?.Message ?? "Failed to assign role",
                            null, sourceCenterId, requestId);

                        continue; // Skip permissions if role creation failed
                    }

                    // ═══════════════════════════════════════════════════════
                    // STEP 2: FETCH ROLES LIST TO GET ROLECODE
                    // ═══════════════════════════════════════════════════════
                    var rolesListResponse = await kidkareService.GetRoleAsync(centerId);

                    if (rolesListResponse?.IsSuccess != true || rolesListResponse.Data == null)
                    {
                        _logger.LogWarning("Could not fetch roles for center {CenterId}", centerId);
                        continue;
                    }

                    // ═══════════════════════════════════════════════════════
                    // STEP 3: FIND THE NEWLY CREATED ROLE AND GET ROLECODE
                    // ═══════════════════════════════════════════════════════
                    var roleInfo = rolesListResponse.Data.FirstOrDefault(r => r.RoleName == roleName);

                    if (roleInfo == null)
                    {
                        _logger.LogWarning("Role '{RoleName}' not found in center {CenterId} roles list", roleName, centerId);
                        continue;
                    }

                    int roleCode = roleInfo.RoleCode;
                    _logger.LogInformation("Found RoleCode {RoleCode} for role '{RoleName}' in center {CenterId}", roleCode, roleName, centerId);

                    // ═══════════════════════════════════════════════════════
                    // STEP 4: UPDATE PERMISSIONS WITH USERID = -ROLECODE
                    // ═══════════════════════════════════════════════════════
                    // CRITICAL: Kidkare uses NEGATIVE RoleCode as UserId for role-based permissions
                    var updatedPermissions = permissions.Select(p =>
                    {
                        p.UserId = -roleCode;  // ← IMPORTANT: Negative value!
                        return p;
                    }).ToList();

                    // ═══════════════════════════════════════════════════════
                    // STEP 5: ASSIGN PERMISSIONS IN PARALLEL (MAX 10)
                    // ═══════════════════════════════════════════════════════
                    const int maxConcurrency = 10;
                    using var semaphore = new SemaphoreSlim(maxConcurrency);

                    var permissionTasks = updatedPermissions.Select(async perm =>
                    {
                        await semaphore.WaitAsync();  // Wait for available slot
                        try
                        {
                            var permResponse = await kidkareService.SavePermissionAsync(perm);

                            if (permResponse?.IsSuccess == true)
                            {
                                _logger.LogInformation("Saved permission '{RightName}' for role '{RoleName}' center {CenterId}", perm.RightName, roleName, centerId);

                                // Log permission creation
                                await LogSyncAsync(tenant, EntityType.Center,
                                    $"{sourceCenterId}_Permission_{roleName}_{perm.RightName}",
                                    null,
                                    SyncAction.Insert, SyncStatus.Success,
                                    $"Permission created: {perm.RightName} for role {roleName}",
                                    null, sourceCenterId, requestId);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to save permission '{RightName}' for role '{RoleName}': {Message}", perm.RightName, roleName, permResponse?.Message);

                                await LogSyncAsync(tenant, EntityType.Center,
                                    $"{sourceCenterId}_Permission_{roleName}_{perm.RightName}",
                                    null,
                                    SyncAction.Error, SyncStatus.Failed,
                                    permResponse?.Message ?? "Failed to save permission",
                                    null, sourceCenterId, requestId);
                            }
                        }
                        catch (HttpRequestException httpEx)
                        {
                            _logger.LogError(httpEx, "HTTP error saving permission '{RightName}' for role '{RoleName}' (Center {CenterId})", perm.RightName, roleName, centerId);

                            await LogSyncAsync(tenant, EntityType.Center,
                                $"{sourceCenterId}_Permission_{roleName}_{perm.RightName}",
                                null,
                                SyncAction.Error, SyncStatus.Failed,
                                $"HTTP error: {httpEx.Message}",
                                null, sourceCenterId, requestId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving permission '{RightName}' for role '{RoleName}' (Center {CenterId})", perm.RightName, roleName, centerId);

                            await LogSyncAsync(tenant, EntityType.Center,
                                $"{sourceCenterId}_Permission_{roleName}_{perm.RightName}",
                                null,
                                SyncAction.Error, SyncStatus.Failed,
                                ex.Message,
                                null, sourceCenterId, requestId);
                        }
                        finally
                        {
                            semaphore.Release();  // Release slot for next permission
                        }
                    });

                    // Wait for all permission assignments to complete
                    await Task.WhenAll(permissionTasks);

                    _logger.LogInformation("Assigned {Count} permissions to role '{RoleName}' at Center {CenterId}", updatedPermissions.Count, roleName, centerId);
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "HTTP error creating role '{RoleName}' at center {CenterId}", roleName, centerId);

                    await LogSyncAsync(tenant, EntityType.Center,
                        $"{sourceCenterId}_Role_{roleName}",
                        null,
                        SyncAction.Error, SyncStatus.Failed,
                        $"HTTP error: {httpEx.Message}",
                        null, sourceCenterId, requestId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to create/assign role '{RoleName}' at Center {CenterId}", roleName, centerId);

                    await LogSyncAsync(tenant, EntityType.Center,
                        $"{sourceCenterId}_Role_{roleName}",
                        null,
                        SyncAction.Error, SyncStatus.Failed,
                        ex.Message,
                        null, sourceCenterId, requestId);
                }
            }

            _logger.LogInformation("Completed creating default roles and permissions for center {CenterId}", centerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default roles and permissions for center {CenterId}", centerId);
        }
    }

    /// <summary>
    /// Sync a single staff member
    /// </summary>
    private async Task<SyncAction> SyncStaffAsync(
        TenantConfiguration tenant,
        ChildPlusStaff staff,
        Guid requestId,
        CenterStaffAddRequest staffRequest,
        IKidkareService kidkareService)
    {
        try
        {
            // Check duplicate
            var shouldSync = await _syncLogRepository.ShouldSyncAsync(
                tenant.KidkareCxSqlConnectionString,
                EntityType.Staff.ToString(),
                staff.StaffId,
                staff.Timestamp);

            if (!shouldSync)
            {
                await LogSyncAsync(tenant, EntityType.Staff, staff.StaffId, null,
                    SyncAction.Skip, SyncStatus.Success, "No changes detected",
                    staff.Timestamp, staff.CenterId, requestId);
                return SyncAction.Skip;
            }

            // call API
            var response = await kidkareService.SaveStaffAsync(staffRequest);

            if (response.IsSuccess)
            {
                await LogSyncAsync(tenant, EntityType.Staff, staff.StaffId,
                    response.Data?.StaffId.ToString(),
                    SyncAction.Update, SyncStatus.Success, response.Message,
                    staff.Timestamp, staff.CenterId, requestId);
                return SyncAction.Update;
            }
            else
            {
                await LogSyncAsync(tenant, EntityType.Staff, staff.StaffId, null,
                    SyncAction.Error, SyncStatus.Failed, response.Message,
                    staff.Timestamp, staff.CenterId, requestId);
                return SyncAction.Error;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing staff {StaffId}", staff.StaffId);
            await LogSyncAsync(tenant, EntityType.Staff, staff.StaffId, null,
                SyncAction.Error, SyncStatus.Failed, ex.Message,
                staff.Timestamp, staff.CenterId, requestId);
            return SyncAction.Error;
        }
    }

    #endregion

    #region Private Methods - Children Level

    /// <summary>
    /// Sync all children across all sites in batches
    /// </summary>
    private async Task<(int success, int failed, int skipped)> SyncAllChildrenInBatchesAsync(
        TenantConfiguration tenant,
        List<ChildPlusSite> sites,
        int batchSize,
        Guid requestId,
        IKidkareService kidkareService)
    {
        int totalSuccess = 0, totalFailed = 0;

        try
        {
            // Step 1: Collect all children with their relations
            var allChildren = await CollectAllChildrenWithRelationsAsync(tenant, sites);

            _logger.LogInformation("Collected {Count} children for tenant {TenantId}", allChildren.Count, tenant.TenantId);

            // Step 2: Filter children that need syncing
            var (childrenToSync, totalSkipped) = await FilterChildrenForSyncAsync(tenant, allChildren);

            if (!childrenToSync.Any())
            {
                _logger.LogInformation("No children need syncing for tenant {TenantId}", tenant.TenantId);
                return (0, 0, totalSkipped);
            }

            _logger.LogInformation("Processing {Count} children in batches of {BatchSize}", childrenToSync.Count, batchSize);

            // Step 3: Process in batches
            var (success, failed) = await ProcessChildrenInBatchesAsync(tenant, childrenToSync, batchSize, requestId, kidkareService, sites);

            totalSuccess = success;
            totalFailed = failed;

            return (totalSuccess, totalFailed, totalSkipped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing children for tenant {TenantId}", tenant.TenantId);
            throw;
        }
    }

    /// <summary>
    /// Collect all children from all sites with their guardians, enrollments, attendance
    /// </summary>
    private async Task<List<ChildPlusChild>> CollectAllChildrenWithRelationsAsync(
        TenantConfiguration tenant,
        List<ChildPlusSite> sites)
    {
        var allChildren = new List<ChildPlusChild>();

        foreach (var site in sites)
        {
            var children = await _childPlusRepository.GetChildrenByCenterIdAsync(tenant.ChildPlusConnectionString, site.CenterId);

            // Load relations for each child
            foreach (var child in children)
            {
                child.Guardians = await _childPlusRepository.GetGuardiansByChildIdAsync(tenant.ChildPlusConnectionString, child.ChildId);

                child.Enrollments = await _childPlusRepository.GetEnrollmentsByChildIdAsync(tenant.ChildPlusConnectionString, child.ChildId);

                child.Attendance = await _childPlusRepository.GetAttendanceByChildIdAsync(tenant.ChildPlusConnectionString, child.ChildId);
            }

            allChildren.AddRange(children);
        }

        return allChildren;
    }

    /// <summary>
    /// Filter children based on timestamp to determine which need syncing
    /// </summary>
    private async Task<(List<ChildPlusChild> ChildrenToSync, int TotalSkipped)> FilterChildrenForSyncAsync(
        TenantConfiguration tenant,
        List<ChildPlusChild> allChildren)
    {
        var childrenToSync = new List<ChildPlusChild>();
        int totalSkipped = 0;

        foreach (var child in allChildren)
        {
            var shouldSync = await _syncLogRepository.ShouldSyncAsync(
                tenant.KidkareCxSqlConnectionString,
                EntityType.Child.ToString(),
                child.ChildId,
                child.Timestamp);

            if (shouldSync)
                childrenToSync.Add(child);
            else
                totalSkipped++;
        }

        return (childrenToSync, totalSkipped);
    }

    /// <summary>
    /// Process children in batches and call FinalizeImport API
    /// </summary>
    private async Task<(int success, int failed)> ProcessChildrenInBatchesAsync(
        TenantConfiguration tenant,
        List<ChildPlusChild> childrenToSync,
        int batchSize,
        Guid requestId,
        IKidkareService kidkareService,
        List<ChildPlusSite> sites)
    {
        int totalSuccess = 0, totalFailed = 0;
        var centerName = sites.FirstOrDefault()?.CenterName ?? tenant.TenantName;

        for (int i = 0; i < childrenToSync.Count; i += batchSize)
        {
            var batch = childrenToSync.Skip(i).Take(batchSize).ToList();
            var batchNumber = (i / batchSize) + 1;

            _logger.LogInformation("Processing batch {BatchNum} with {Count} children", batchNumber, batch.Count);

            try
            {
                // Map with row numbers for tracking
                var kidkareChildren = batch.Select((child, index) => _dataMapper.MapToKidkareChild(child, i + index + 1)).ToList();

                // Call API
                var response = await kidkareService.FinalizeImportAsync(kidkareChildren, centerName);

                // Process response and log
                var (success, failed) = await ProcessBatchResponseAsync(tenant, batch, response, requestId);

                totalSuccess += success;
                totalFailed += failed;

                _logger.LogInformation("Batch {BatchNum} completed: {Success} success, {Failed} failed", batchNumber, success, failed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch {BatchNum}", batchNumber);

                // Log all in batch as failed
                await LogBatchAsFailedAsync(tenant, batch, ex.Message, requestId);
                totalFailed += batch.Count;
            }
        }

        return (totalSuccess, totalFailed);
    }

    /// <summary>
    /// Process batch response and create sync logs
    /// </summary>
    private async Task<(int success, int failed)> ProcessBatchResponseAsync(
        TenantConfiguration tenant,
        List<ChildPlusChild> batch,
        ResponseWithData<List<ParseResult<CxChildModel>>> response,
        Guid requestId)
    {
        int success = 0, failed = 0;
        var logs = new List<SyncLog>();

        if (response.IsSuccess && response.Data != null)
        {
            for (int j = 0; j < batch.Count; j++)
            {
                var child = batch[j];
                var parseResult = j < response.Data.Count ? response.Data[j] : null;

                // Check if child import succeeded (no errors and has Id)
                bool hasErrors = parseResult?.Errors != null && parseResult.Errors.Any();
                bool hasId = !string.IsNullOrEmpty(parseResult?.Result?.Id.ToString());
                bool isSuccess = !hasErrors && hasId;

                // Build error message from validation errors
                string errorMessage = "";
                if (hasErrors)
                {
                    errorMessage = string.Join("; ", parseResult.Errors.Select(e => $"{e.ColumnName}: {e.Errors}"));
                }

                logs.Add(CreateSyncLog(
                    tenant,
                    EntityType.Child,
                    child.ChildId,
                    parseResult?.Result?.Id.ToString(),
                    isSuccess ? SyncAction.Update : SyncAction.Error,
                    isSuccess ? SyncStatus.Success : SyncStatus.Failed,
                    isSuccess ? "Child synced successfully" : $"Failed: {errorMessage}",
                    child.Timestamp,
                    child.CenterId,
                    requestId));

                if (isSuccess) success++; else failed++;
            }
        }
        else
        {
            // API call failed - log all as failed
            logs = batch.Select(child => CreateSyncLog(
                tenant, EntityType.Child, child.ChildId, null,
                SyncAction.Error, SyncStatus.Failed,
                response.Message ?? "Batch import failed",
                child.Timestamp, child.CenterId, requestId)
            ).ToList();

            failed = batch.Count;
        }

        await _syncLogRepository.InsertBatchSyncLogsAsync(tenant.KidkareCxSqlConnectionString, logs);

        return (success, failed);
    }

    /// <summary>
    /// Log entire batch as failed when exception occurs
    /// </summary>
    private async Task LogBatchAsFailedAsync(
        TenantConfiguration tenant,
        List<ChildPlusChild> batch,
        string errorMessage,
        Guid requestId)
    {
        var logs = batch.Select(child => CreateSyncLog(
            tenant, EntityType.Child, child.ChildId, null,
            SyncAction.Error, SyncStatus.Failed, errorMessage,
            child.Timestamp, child.CenterId, requestId)
        ).ToList();

        await _syncLogRepository.InsertBatchSyncLogsAsync(tenant.KidkareCxSqlConnectionString, logs);
    }

    #endregion

    #region Private Methods - Logging

    /// <summary>
    /// Log single sync operation
    /// </summary>
    private async Task LogSyncAsync(
        TenantConfiguration tenant,
        EntityType entityType,
        string sourceId,
        string targetId,
        SyncAction action,
        SyncStatus status,
        string message,
        byte[] timestamp,
        string centerId,
        Guid requestId)
    {
        var log = CreateSyncLog(tenant, entityType, sourceId, targetId, action, status, message, timestamp, centerId, requestId);

        await _syncLogRepository.InsertSyncLogAsync(tenant.KidkareCxSqlConnectionString, log);
    }

    /// <summary>
    /// Create SyncLog object
    /// </summary>
    private SyncLog CreateSyncLog(
        TenantConfiguration tenant,
        EntityType entityType,
        string sourceId,
        string targetId,
        SyncAction action,
        SyncStatus status,
        string message,
        byte[] timestamp,
        string centerId,
        Guid requestId)
    {
        return new SyncLog
        {
            EntityType = entityType.ToString(),
            SourceId = sourceId,
            TargetId = targetId,
            SyncAction = action.ToString(),
            SyncStatus = status.ToString(),
            Message = message ?? string.Empty,
            TimestampChildPlus = ConvertTimestampToDateTime(timestamp),
            CenterId = centerId,
            RequestId = requestId,
            CreatedBy = SyncConstants.SystemName
        };
    }

    /// <summary>
    /// Convert SQL Server ROWVERSION to DateTime
    /// </summary>
    private DateTime? ConvertTimestampToDateTime(byte[] timestamp)
    {
        if (timestamp == null || timestamp.Length != 8)
            return null;

        try
        {
            var ticks = BitConverter.ToInt64(timestamp.Reverse().ToArray(), 0);
            return new DateTime(1900, 1, 1).AddTicks(ticks);
        }
        catch
        {
            return null;
        }
    }

    #endregion
}