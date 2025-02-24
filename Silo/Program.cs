using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Grains.Services;
using Grains.Configuration;
using Grains;  // Add this to reference HelloGrain
using Orleans.Hosting;  // Add this to resolve ConfigureApplicationParts

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args);
    })
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .UseLocalhostClustering(
                siloPort: 11111,
                gatewayPort: 30000,
                primarySiloEndpoint: null,
                serviceId: "OrleansBasicSiloService",
                clusterId: "dev"
            )
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "OrleansBasicSiloService";
            })   
            
            .AddMemoryGrainStorage("PubSubStore")
            .ConfigureLogging(logging => logging
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information)
            );
    })
    .ConfigureServices((context, services) =>
    {
        // Configure RabbitMQ options
        services.Configure<RabbitMQOptions>(
            context.Configuration.GetSection("RabbitMQ"));

        // Register RabbitMQ service
        services.AddSingleton<IRabbitMQService, RabbitMQService>();

        // Configure logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Information);
        });
    });

try
{
    var host = builder.Build();

    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting Orleans Silo...");

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Critical error starting Orleans silo: {ex}");
    throw;
}