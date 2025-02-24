using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
	public class HelloGrain : Grain, IHello
	{
		private readonly ILogger<HelloGrain> _logger;

		public HelloGrain(ILogger<HelloGrain> logger)
		{
			_logger = logger;
		}

		public ValueTask<string> SayHello(string greeting)
		{
			_logger.LogInformation(
				"SayHello message received: greeting = {Greeting}",
				greeting);

			return ValueTask.FromResult(
				$"""
                Client said: "{greeting}", so HelloGrain says: Hello!
                """);
		}
	}
}
