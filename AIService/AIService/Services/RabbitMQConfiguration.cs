namespace AIService.Services;

public class RabbitMQConfiguration
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "ai-processing-queue";
    public bool Durable { get; set; } = true;
    public bool AutoAck { get; set; } = false;
}
