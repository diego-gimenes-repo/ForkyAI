namespace Grains.Configuration;

public class RabbitMQOptions
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
    public string QueueName { get; set; } = "streamQueue";
}
