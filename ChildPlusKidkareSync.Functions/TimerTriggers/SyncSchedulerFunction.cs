using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChildPlusKidkareSync.Functions.TimerTriggers;

public class SyncSchedulerFunction
{
    private readonly ILogger<SyncSchedulerFunction> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public SyncSchedulerFunction(
        ILogger<SyncSchedulerFunction> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }

    [Function("ChildPlusSyncScheduler")]
    public async Task Run(
        [TimerTrigger("%SyncConfiguration:CronSchedule%", RunOnStartup = true)] TimerInfo timerInfo)
    {
        _logger.LogInformation("SyncScheduler timer triggered at: {Time}", DateTime.UtcNow);

        try
        {
            // Get the function app base URL
            var functionUrl = Environment.GetEnvironmentVariable("FUNCTION_APP_URL");
            var syncEndpoint = $"{functionUrl}/ok-school/childplus-integration";

            _logger.LogInformation("Calling HTTP trigger at: {Endpoint}", syncEndpoint);

            // Call the HTTP trigger
            var response = await _httpClient.PostAsync(syncEndpoint, null);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Sync completed successfully: {Response}", content);
            }
            else
            {
                _logger.LogError("Sync failed with status code: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SyncScheduler");
            throw;
        }

        _logger.LogInformation("Next timer schedule: {Next}", timerInfo.ScheduleStatus?.Next);
    }
}