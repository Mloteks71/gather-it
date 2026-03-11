using AIService.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AIService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAIService _aiService;
    private readonly RabbitMQConfiguration _rabbitConfig;
    private IConnection? _connection;
    private IChannel? _channel;

    public Worker(
        ILogger<Worker> logger,
        IAIService aiService,
        IOptions<RabbitMQConfiguration> rabbitConfig)
    {
        _logger = logger;
        _aiService = aiService;
        _rabbitConfig = rabbitConfig.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connecting to RabbitMQ at {HostName}:{Port}", _rabbitConfig.HostName, _rabbitConfig.Port);

        var factory = new ConnectionFactory
        {
            HostName = _rabbitConfig.HostName,
            Port = _rabbitConfig.Port,
            UserName = _rabbitConfig.UserName,
            Password = _rabbitConfig.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _rabbitConfig.QueueName,
            durable: _rabbitConfig.Durable,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Connected to RabbitMQ and declared queue: {QueueName}", _rabbitConfig.QueueName);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("Channel is not initialized");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message from RabbitMQ: {Message}", message);

                // Process the message with AI service
                var results = await _aiService.ProcessMessageAsync(message, stoppingToken);

                _logger.LogInformation("AI service processed message and returned {Count} results", results.Count);

                // Process the results as needed
                foreach (var result in results)
                {
                    // TODO: Handle each parsed message (e.g., save to database, send to another queue, etc.)
                    _logger.LogInformation("Processed result: {Result}", System.Text.Json.JsonSerializer.Serialize(result));
                }

                // Acknowledge the message
                if (!_rabbitConfig.AutoAck)
                {
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from RabbitMQ");
                
                // Reject and requeue the message
                if (!_rabbitConfig.AutoAck && _channel != null)
                {
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _rabbitConfig.QueueName,
            autoAck: _rabbitConfig.AutoAck,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Started consuming messages from queue: {QueueName}", _rabbitConfig.QueueName);

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
