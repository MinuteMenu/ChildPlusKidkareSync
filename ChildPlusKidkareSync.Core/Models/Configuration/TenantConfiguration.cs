namespace ChildPlusKidkareSync.Core.Models.Configuration;

public class TenantConfiguration
{
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public string TenantName { get; set; }
    public string ChildPlusConnectionString { get; set; }
    public string KidkareApiBaseUrl { get; set; }
    public string KidkareApiKey { get; set; }
    public string KidkareCxSqlConnectionString { get; set; }
    public string KidkareHxSqlConnectionString { get; set; }
    public bool Enabled { get; set; }
}

public class SyncConfiguration
{
    public string CronSchedule { get; set; }
    public int BatchSize { get; set; }
    public int MaxParallelTenants { get; set; }
    public int MaxParallelSites { get; set; }
    public int RateLimitPerMinute { get; set; }
    public int RetryAttempts { get; set; }
    public int RetryDelaySeconds { get; set; }
}