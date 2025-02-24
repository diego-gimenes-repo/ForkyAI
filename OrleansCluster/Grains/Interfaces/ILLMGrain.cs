using Orleans;
using System.Threading.Tasks;

namespace OrleansCluster.Grains.Interfaces
{
    public interface ILLMGrain : IGrainWithStringKey
    {
        Task<string> ProcessMessageAsync(string message, string model);
    }
}
