using AIService.Application.Models;

namespace AIService.Application.Interfaces;

public interface IAIService
{
    Task<List<ParsedMessage>> ProcessMessageAsync(string message, CancellationToken cancellationToken = default);
}
