using System;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.Streams; // Add this using directive
using Orleans.Streams; // Add this using directive

namespace OrleansCluster.Infrastructure
{
    internal static class OrleansConfigurationHelpers
    {
        public static void AddOrleansCluster(ISiloHostBuilder siloHostBuilder)
        {
            siloHostBuilder.UseLocalhostClustering() // Use the parameterless overload
                    .AddMemoryGrainStorageAsDefault()
                    .AddMemoryGrainStorage("PubSubStore") // Required for Persistent Streams
                    .AddPersistentStreams("RabbitMQStreamProvider", RabbitMqAdapterFactory.Create<DefaultBatchContainer>, options =>
                    {
                        options.ConfigureRabbitMQ(ob =>
                        {
                            ob.ConnectionString = "amqp://guest:guest@localhost:5672/";
                            ob.ExchangeName = "orleans-exchange";
                        });
                    });
        }
    }
}
