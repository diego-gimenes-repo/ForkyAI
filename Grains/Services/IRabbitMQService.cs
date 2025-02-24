using System;

namespace Grains.Services
{
    public interface IRabbitMQService : IDisposable
    {
        void PublishMessage(string message, string model);
        void SubscribeToMessages(Action<string> messageHandler);
        bool IsConnected { get; }
    }
}
