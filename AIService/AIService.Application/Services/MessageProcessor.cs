using AIService.Application.Interfaces;
using AIService.Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AIService.Application.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(ILogger<MessageProcessor> logger)
    {
        _logger = logger;
    }

    public async Task ProcessMessagesAsync(List<ParsedMessage> messages, CancellationToken cancellationToken = default)
    {
        foreach (var message in messages)
        {
            // TODO: Implement your business logic here
            // Examples:
            // - Save to database
            // - Send to another service
            // - Validate and transform data
            // - Publish to another queue
            
            _logger.LogInformation("Processing message: {Message}", JsonSerializer.Serialize(message));
            
            await Task.CompletedTask;
        }
    }
}
