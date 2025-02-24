using Orleans;
using Orleans.Streams;
using System.Threading.Tasks;

namespace OrleansCluster.Abstractions.Grains
{
    public interface ILLMStreamGrain : IGrainWithGuidKey
    {
        Task SubscribeToStream();
        Task PublishMessage(string message, string model);
    }
}
