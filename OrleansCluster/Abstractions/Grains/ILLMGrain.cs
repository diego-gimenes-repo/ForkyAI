using Orleans;
using System.Threading.Tasks;

namespace OrleansCluster.Abstractions.Grains
{
    public interface ILLMGrain : IGrainWithStringKey
    {
        Task<string> ProcessMessageAsync(string message, string model);
    }
}
