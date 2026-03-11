using AIService.Application.Models;

namespace AIService.Application.Interfaces;

public interface IMessageProcessor
{
    Task ProcessMessagesAsync(List<ParsedMessage> messages, CancellationToken cancellationToken = default);
}
