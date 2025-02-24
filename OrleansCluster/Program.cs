using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansCluster.Infrastructure;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione referências necessárias
        builder.Services.AddOrleansClient(clientBuilder =>
        {
            clientBuilder.UseLocalhostClustering()
                         .Configure<ClusterOptions>(options =>
                         {
                             options.ClusterId = "dev";
                             options.ServiceId = "MyService";
                         });
        });

        // Configure Orleans Silo
        builder.Host.UseOrleans((ctx, siloBuilder) =>
        {
            // Use local clustering for development
            siloBuilder.UseLocalhostClustering();

            // Register all grains from the application assembly

        });

        // Register Orleans Cluster Client for communication with grains
        builder.Services.AddSingleton(sp =>
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "MyService";
                })
                .Build();

            client.Connect().Wait();
            return client;
        });

        // Register HttpClient for external API calls (e.g., Semantic Kernel)
        builder.Services.AddHttpClient();

        // Register controllers & Swagger for API documentation
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}