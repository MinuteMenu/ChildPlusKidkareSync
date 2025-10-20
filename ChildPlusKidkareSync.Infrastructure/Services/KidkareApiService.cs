using ChildPlusKidkareSync.Core.Constants;
using ChildPlusKidkareSync.Core.Models.Kidkare;
using ChildPlusKidkareSync.Core.Models.Sync;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChildPlusKidkareSync.Infrastructure.Services;

// ==================== KIDKARE API SERVICE ====================
public interface IKidkareService
{
    Task<ResponseWithData<object>> SaveCenterAsync(CenterSaveRequest center, CancellationToken cancellationToken = default);
    Task<ResponseWithData<List<RoleModel>>> GetRoleAsync(int centerId, CancellationToken cancellationToken = default);
    Task<ResponseWithData<RoleAddResponse>> AssignRoleAsync(RoleModel role, CancellationToken cancellationToken = default);
    Task<ResponseWithData<object>> SavePermissionAsync(SaveStaffPermissionRequest perm, CancellationToken cancellationToken = default);
    Task<ResponseWithData<CenterStaffModel>> SaveStaffAsync(CenterStaffAddRequest staff, CancellationToken cancellationToken = default);
    Task<ResponseWithData<List<ParseResult<CxChildModel>>>> FinalizeImportAsync(List<ParseResult<CxChildModel>> children, string centerName, CancellationToken cancellationToken = default);
}

public class KidkareService : IKidkareService
{
    private readonly KidkareClient _client;
    private readonly ILogger<KidkareService> _logger;

    public KidkareService(KidkareClient client, ILogger<KidkareService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ResponseWithData<object>> SaveCenterAsync(
        CenterSaveRequest center,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var centerName = center?.CenterModel?.General?.CenterInfo?.CenterName ?? "Unknown";
            _logger.LogInformation("Creating center: {CenterName}", centerName);

            var jsonPayload = JsonHelper.PreparePayload(center);
            var result = await _client.PostAsync(SyncConstants.ApiEndpoints.SaveCenter, jsonPayload, cancellationToken);

            var response = ResponseWithData<object>.Success(JsonConvert.DeserializeObject<object>(result));

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully created center: {CenterName}", centerName);
            }
            else
            {
                _logger.LogWarning("Failed to create center {CenterName}: {Message}",
                    centerName, response?.Message);
            }

            return response ?? ResponseWithData<object>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error saving center {CenterName}: {Message}",
                center?.CenterModel?.General?.CenterInfo?.CenterName, httpEx.Message);
            return ResponseWithData<object>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving center {CenterName}: {Message}",
                center?.CenterModel?.General?.CenterInfo?.CenterName, ex.Message);
            return ResponseWithData<object>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<List<RoleModel>>> GetRoleAsync(
     int centerId,
     CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching roles for centerId: {CenterId}", centerId);

            var endpoint = $"{SyncConstants.ApiEndpoints.GetRole}?withPermissions=false&centerId={centerId}";
            var result = await _client.GetAsync(endpoint, cancellationToken: cancellationToken);
            var roleResponse = JsonConvert.DeserializeObject<RolesListResponse>(result);
            var response = ResponseWithData<List<RoleModel>>.Success(roleResponse?.Roles);

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully fetched {Count} roles for centerId: {CenterId}",
                    roleResponse?.Roles?.Count ?? 0, centerId);
            }
            else
            {
                _logger.LogWarning("Failed to fetch roles for centerId: {CenterId}. Message: {Message}",
                    centerId, response?.Message);
            }

            return response ?? ResponseWithData<List<RoleModel>>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error fetching roles for centerId: {CenterId}: {Message}",
                centerId, httpEx.Message);
            return ResponseWithData<List<RoleModel>>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching roles for centerId: {CenterId}: {Message}",
                centerId, ex.Message);
            return ResponseWithData<List<RoleModel>>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<RoleAddResponse>> AssignRoleAsync(
        RoleModel role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Assigning role: {RoleName} to centerId: {CenterId}",
                role?.RoleName ?? "Unknown", role?.CenterId);

            var result = await _client.PutAsync(SyncConstants.ApiEndpoints.AddRole, role, cancellationToken);
            var response = ResponseWithData<RoleAddResponse>.Success(JsonConvert.DeserializeObject<RoleAddResponse>(result));

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully assigned role: {RoleName} to centerId: {CenterId}",
                    role?.RoleName, role?.CenterId);
            }
            else
            {
                _logger.LogWarning("Failed to assign role: {RoleName} to centerId: {CenterId}. Message: {Message}",
                    role?.RoleName, role?.CenterId, response?.Message);
            }

            return response ?? ResponseWithData<RoleAddResponse>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error assigning role: {RoleName} to centerId: {CenterId}: {Message}",
                role?.RoleName, role?.CenterId, httpEx.Message);
            return ResponseWithData<RoleAddResponse>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error assigning role: {RoleName} to centerId: {CenterId}: {Message}",
                role?.RoleName, role?.CenterId, ex.Message);
            return ResponseWithData<RoleAddResponse>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<object>> SavePermissionAsync(
        SaveStaffPermissionRequest perm,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving permission: {RightName}", perm?.RightName ?? "Unknown");

            var result = await _client.PostAsync(SyncConstants.ApiEndpoints.SavePermission, perm, cancellationToken);
            var response = ResponseWithData<object>.Success(JsonConvert.DeserializeObject<object>(result));

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully saved permission: {RightName}", perm?.RightName);
            }
            else
            {
                _logger.LogWarning("Failed to save permission: {RightName}. Message: {Message}",
                    perm?.RightName, response?.Message);
            }

            return response ?? ResponseWithData<object>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error saving permission: {RightName}: {Message}",
                perm?.RightName, httpEx.Message);
            return ResponseWithData<object>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving permission: {RightName}: {Message}",
                perm?.RightName, ex.Message);
            return ResponseWithData<object>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<CenterStaffModel>> SaveStaffAsync(
        CenterStaffAddRequest staff,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating staff: {FirstName} {LastName}",
                staff?.FirstName, staff?.LastName);

            var result = await _client.PostAsync(
                SyncConstants.ApiEndpoints.AddStaff,
                staff,
                cancellationToken);

            var response = JsonConvert.DeserializeObject<ResponseWithData<CenterStaffModel>>(result);

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully created staff: {FirstName} {LastName}",
                    staff?.FirstName, staff?.LastName);
            }
            else
            {
                _logger.LogWarning("Failed to create staff {FirstName} {LastName}: {Message}",
                    staff?.FirstName, staff?.LastName, response?.Message);
            }

            return response ?? ResponseWithData<CenterStaffModel>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error saving staff {FirstName} {LastName}: {Message}",
                staff?.FirstName, staff?.LastName, httpEx.Message);
            return ResponseWithData<CenterStaffModel>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving staff {FirstName} {LastName}: {Message}",
                staff?.FirstName, staff?.LastName, ex.Message);
            return ResponseWithData<CenterStaffModel>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<object>> UpdateStaffAsync(CenterStaffUpdateRequest staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating staff: {Staff}", staff);

            var result = await _client.PostAsync(SyncConstants.ApiEndpoints.UpdateStaff, staff, cancellationToken);
            var response = ResponseWithData<object>.Success(JsonConvert.DeserializeObject<object>(result));

            if (response?.IsSuccess == true)
            {
                _logger.LogInformation("Successfully updated staff: {Staff}", staff);
            }
            else
            {
                _logger.LogWarning("Failed to update staff: {Staff}. Message: {Message}", staff, response?.Message);
            }

            return response ?? ResponseWithData<object>.Fail("Empty response from API");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error updating staff: {Staff}: {Message}", staff, httpEx.Message);
            return ResponseWithData<object>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating staff: {Staff}: {Message}", staff, ex.Message);
            return ResponseWithData<object>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseWithData<List<ParseResult<CxChildModel>>>> FinalizeImportAsync(
        List<ParseResult<CxChildModel>> children,
        string centerName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Finalizing import of {Count} children for center {CenterName}", children?.Count ?? 0, centerName);

            var jsonPayload = JsonHelper.PreparePayload(children);
            var result = await _client.PostAsync(
                SyncConstants.ApiEndpoints.FinalizeImport,
                jsonPayload,
                cancellationToken);

            var response = JsonConvert.DeserializeObject<List<ParseResult<CxChildModel>>>(result);

            if (response != null)
            {
                // Count successful imports (no errors)
                var successCount = response.Count(r => r.Errors == null || r.Errors.Count == 0);
                var failedCount = response.Count - successCount;

                _logger.LogInformation("Finalized import for center {CenterName}: {Success}/{Total} succeeded, {Failed} failed",
                    centerName, successCount, response.Count, failedCount);

                return ResponseWithData<List<ParseResult<CxChildModel>>>.Success(response, $"Processed {response.Count} children: {successCount} succeeded, {failedCount} failed");
            }
            else
            {
                _logger.LogWarning("Empty response from FinalizeImport for center {CenterName}", centerName);
                return ResponseWithData<List<ParseResult<CxChildModel>>>.Fail("Empty response from API");
            }


            //var response = ParseFinalizeImportResponse(result, children);

            //if (response?.IsSuccess == true)
            //{
            //    var successCount = response.Data?.Count(c => c.Result.Id > 0) ?? 0;
            //    _logger.LogInformation("Successfully finalized import for center {CenterName}: {Success}/{Total} children", centerName, successCount, children?.Count ?? 0);
            //}
            //else
            //{
            //    _logger.LogWarning("Failed to finalize import for center {CenterName}: {Message}", centerName, response?.Message);
            //}

            //return response;
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error finalizing import for center {CenterName}", centerName);
            return ResponseWithData<List<ParseResult<CxChildModel>>>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing import for center {CenterName}", centerName);
            return ResponseWithData<List<ParseResult<CxChildModel>>>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    private ResponseWithData<List<ParseResult<CxChildModel>>> ParseFinalizeImportResponse(
        string jsonResponse,
        List<ParseResult<CxChildModel>> originalRequests)
    {
        try
        {
            // Deserialize API response
            var apiResponse = JsonConvert.DeserializeObject<ResponseWithData<List<ParseResult<CxChildModel>>>>(jsonResponse);

            if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
            {
                // Map response IDs back to original requests
                for (int i = 0; i < originalRequests.Count && i < apiResponse.Data.Count; i++)
                {
                    if (apiResponse.Data[i].Result.Id > 0)
                    {
                        originalRequests[i].Result.Id = apiResponse.Data[i].Result.Id;
                    }
                }

                return ResponseWithData<List<ParseResult<CxChildModel>>>.Success(
                    originalRequests,
                    apiResponse.Message);
            }

            return apiResponse ?? ResponseWithData<List<ParseResult<CxChildModel>>>.Fail("Empty response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing finalize import response");
            return ResponseWithData<List<ParseResult<CxChildModel>>>.Fail($"Parse error: {ex.Message}");
        }
    }
}