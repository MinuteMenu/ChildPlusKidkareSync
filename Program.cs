using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChildPlusKidkareSync.Core.Models.Configuration;
using ChildPlusKidkareSync.Infrastructure.Data;
using ChildPlusKidkareSync.Infrastructure.Services;
using ChildPlusKidkareSync.Infrastructure.Mapping;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults() // Azure Functions Isolated

    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        // Load config to flatten nested SyncConfiguration for TimerTrigger
        var builtConfig = config.Build();
        var syncSection = builtConfig.GetSection("SyncConfiguration");

        foreach (var kv in syncSection.GetChildren())
        {
            Environment.SetEnvironmentVariable($"SyncConfiguration:{kv.Key}", kv.Value);
        }
    })

    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Bind configuration sections to models
        services.Configure<SyncConfiguration>(configuration.GetSection("SyncConfiguration"));
        services.Configure<List<TenantConfiguration>>(configuration.GetSection("Tenants"));

        // Logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        // Register repositories
        services.AddScoped<IChildPlusRepository, ChildPlusRepository>();
        services.AddScoped<ISyncLogRepository, SyncLogRepository>();

        // Register mappers
        services.AddScoped<IDataMapper, DataMapper>();

        // Register KidKare services — without hardcoded placeholder
        services.AddScoped<IKidkareService, KidkareService>();
        services.AddScoped<KidkareClient>();

        // Sync orchestration
        services.AddScoped<ISyncService, SyncService>();

        // HTTP client factory
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
