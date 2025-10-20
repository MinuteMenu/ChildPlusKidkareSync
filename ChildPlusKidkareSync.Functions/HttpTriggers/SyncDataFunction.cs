using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChildPlusKidkareSync.Infrastructure.Services;
using ChildPlusKidkareSync.Core.Models.Configuration;

namespace ChildPlusKidkareSync.Functions.HttpTriggers;

public class SyncDataFunction
{
    private readonly ILogger<SyncDataFunction> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISyncService _syncService;

    public SyncDataFunction(
        ILogger<SyncDataFunction> logger,
        IConfiguration configuration,
        ISyncService syncService)
    {
        _logger = logger;
        _configuration = configuration;
        _syncService = syncService;
    }

    [Function("ChildPlusSyncData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ok-school/childplus-integration")] HttpRequestData req)
    {
        _logger.LogInformation("SyncData HTTP trigger function started at: {Time}", DateTime.UtcNow);

        try
        {
            // Load tenant configurations
            var tenants = _configuration.GetSection("Tenants")
                .Get<List<TenantConfiguration>>() ?? new List<TenantConfiguration>();

            if (!tenants.Any())
            {
                _logger.LogWarning("No tenants configured");
                return await CreateResponse(req, HttpStatusCode.BadRequest, new { error = "No tenants configured" });
            }

            // Load sync configuration
            var syncConfig = _configuration.GetSection("SyncConfiguration")
                .Get<SyncConfiguration>() ?? new SyncConfiguration();

            _logger.LogInformation("Starting sync for {Count} tenants", tenants.Count);

            // Execute sync
            var results = await _syncService.SyncAllTenantsAsync(tenants, syncConfig);

            // Prepare response
            var summary = new
            {
                TotalTenants = results.Count,
                SuccessfulTenants = results.Count(r => r.IsSuccess),
                FailedTenants = results.Count(r => !r.IsSuccess),
                TotalRecordsProcessed = results.Sum(r => r.TotalRecords),
                TotalSuccessRecords = results.Sum(r => r.SuccessCount),
                TotalFailedRecords = results.Sum(r => r.FailedCount),
                TotalSkippedRecords = results.Sum(r => r.SkippedCount),
                StartTime = results.Min(r => r.StartTime),
                EndTime = results.Max(r => r.EndTime),
                DurationSeconds = (results.Max(r => r.EndTime) - results.Min(r => r.StartTime)).TotalSeconds,
                TenantResults = results.Select(r => new
                {
                    r.TenantId,
                    r.RequestId,
                    r.TotalRecords,
                    r.SuccessCount,
                    r.FailedCount,
                    r.SkippedCount,
                    r.IsSuccess,
                    DurationSeconds = (r.EndTime - r.StartTime).TotalSeconds,
                    Errors = r.Errors
                }).ToList()
            };

            _logger.LogInformation("Sync completed - Processed: {Total}, Success: {Success}, Failed: {Failed}, Skipped: {Skipped}",
                summary.TotalRecordsProcessed, summary.TotalSuccessRecords,
                summary.TotalFailedRecords, summary.TotalSkippedRecords);

            return await CreateResponse(req, HttpStatusCode.OK, summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SyncData function");
            return await CreateResponse(req, HttpStatusCode.InternalServerError, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    private static async Task<HttpResponseData> CreateResponse(HttpRequestData req, HttpStatusCode statusCode, object data)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(data);
        return response;
    }
}