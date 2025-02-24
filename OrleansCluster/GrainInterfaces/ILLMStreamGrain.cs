
using Orleans;

namespace GrainInterfaces
{
    public interface ILLMStreamGrain : IGrainWithGuidKey
    {
        Task SubscribeToStream();
        Task PublishMessage(string message, string model);
    }
}
