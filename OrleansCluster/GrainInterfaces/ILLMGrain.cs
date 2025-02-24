using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ILLMGrain : IGrainWithStringKey
    {
        ValueTask<string> ProcessMessageAsync(string message, string model);
    }
}
