using GrainInterfaces;
using Grains.Services;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class LLMStreamGrain : Grain, ILLMStreamGrain, IDisposable
    {
        private readonly ILogger<LLMStreamGrain> _logger;
        private readonly IRabbitMQService _rabbitMQService;
        private bool _disposed;

        public LLMStreamGrain(
            ILogger<LLMStreamGrain> logger,
            IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "LLMStreamGrain activated with ID: {GrainId}",
                this.GetPrimaryKeyString());

            return base.OnActivateAsync(cancellationToken);
        }


        public Task SubscribeToStream()
        {
            // Subscribe to messages from RabbitMQ
            _rabbitMQService.SubscribeToMessages(message =>
            {
                _logger.LogInformation(
                    "Received message in LLMStreamGrain: {Message}",
                    message);
            });

            return Task.CompletedTask;
        }

        public Task PublishMessage(string message, string model)
        {
            _rabbitMQService.PublishMessage(message, model);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            // Perform any additional cleanup if needed

            GC.SuppressFinalize(this);
        }
    }
}
