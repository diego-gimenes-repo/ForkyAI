using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansCluster.Abstractions.Grains;
using System.Threading.Tasks;

namespace OrleansCluster.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LLMController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public LLMController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessMessage([FromBody] LLMRequest request)
        {
            var grain = _grainFactory.GetGrain<ILLMGrain>(request.Model);
            var result = await grain.ProcessMessageAsync(request.Message, request.Model);
            return Ok(result);
        }
    }

    public class LLMRequest
    {
        public string Message { get; set; }
        public string Model { get; set; }
    }
}
