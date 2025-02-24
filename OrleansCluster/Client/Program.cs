using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

using GrainInterfaces;

namespace Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Build the client host.
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .UseOrleansClient(client =>
                {
                    // Adjust cluster connection as needed
                    client.UseLocalhostClustering();
                })
                .ConfigureLogging(logging =>
                {
                    // Prevent duplicate log messages in some environments
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                // Keeps the app alive until you explicitly stop it or press Ctrl + C
                .UseConsoleLifetime();

            using var host = hostBuilder.Build();

            try
            {
                await host.StartAsync();

                // Resolve the Orleans client
                var client = host.Services.GetRequiredService<IClusterClient>();

                // --------------------------------------------
                // Example: Interacting with ILLMGrain
                // --------------------------------------------
                // Note: "my-llm-grain-key" is the string key for your grain instance
                var llmGrain = client.GetGrain<ILLMGrain>("my-llm-grain-key");
                var llmResponse = await llmGrain.ProcessMessageAsync(
                    "Hello from the client!",
                    "gpt-3.5" // or any model name
                );

                Console.WriteLine("LLMGrain Response:");
                Console.WriteLine(llmResponse);

                // --------------------------------------------
                // Example: Interacting with ILLMStreamGrain
                // --------------------------------------------
                // Each ILLMStreamGrain is identified by a GUID key.
                // Use a stable GUID if you want the same grain each time, or a new GUID for each run.
                var llmStreamGrain = client.GetGrain<ILLMStreamGrain>(Guid.NewGuid());

                // Subscribe to the grain’s stream
                await llmStreamGrain.SubscribeToStream();

                // Publish a message to the stream
                await llmStreamGrain.PublishMessage("Message from client stream", "gpt-3.5");

                Console.WriteLine("Message published to ILLMStreamGrain.");

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex}");
            }
            finally
            {
                // Ensure a graceful shutdown.
                await host.StopAsync();
            }
        }
    }
}
