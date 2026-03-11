using AIService.Models;

namespace AIService.Services;

public interface IAIService
{
    Task<List<ParsedMessage>> ProcessMessageAsync(string message, CancellationToken cancellationToken = default);
}
