
using Orleans.Hosting;

using RabbitMQ.Client;
using ISiloBuilder = Orleans.Hosting.ISiloBuilder; 

namespace OrleansCluster.Infrastructure
{
    public static class OrleansConfiguration
    {
        public static void AddOrleansCluster(this ISiloBuilder builder)
        {
            builder.UseLocalhostClustering()
                   .AddMemoryGrainStorageAsDefault() // Set default in-memory storage  
                   .AddMemoryGrainStorage("PubSubStore") // Required for Orleans Streams  
                   .AddPersistentStreams("RabbitMQStreamProvider", (sp, name, options) =>
                   {
                       var factory = new ConnectionFactory
                       {
                           Uri = new Uri("amqp://guest:guest@localhost:5672/")
                       };
                       return Task.FromResult<IQueueAdapterFactory>(new RabbitMQAdapterFactory(factory, options));
                   });
        }
    }
}