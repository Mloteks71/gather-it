using AIService.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Json;

namespace AIService.Services;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly AIConfiguration _config;

    public OpenAIService(
        HttpClient httpClient,
        ILogger<OpenAIService> logger,
        IOptions<AIConfiguration> config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config.Value;
    }

    public async Task<List<ParsedMessage>> ProcessMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPrompt = $"{_config.SystemPrompt}\n\nUser message: {message}";

            var requestBody = new
            {
                model = _config.Model,
                messages = new[]
                {
                    new { role = "system", content = _config.SystemPrompt },
                    new { role = "user", content = message }
                },
                temperature = 0.7
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = JsonContent.Create(requestBody)
            };

            request.Headers.Add("Authorization", $"Bearer {_config.ApiKey}");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(responseContent);

            var aiResponse = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            _logger.LogInformation("AI Response: {Response}", aiResponse);

            // Extract JSON from the response (it might be wrapped in markdown or text)
            var jsonArray = ExtractJsonArray(aiResponse);

            if (string.IsNullOrEmpty(jsonArray))
            {
                _logger.LogWarning("No valid JSON found in AI response");
                return new List<ParsedMessage>();
            }

            var parsedMessages = JsonSerializer.Deserialize<List<ParsedMessage>>(jsonArray);
            return parsedMessages ?? new List<ParsedMessage>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message with AI service");
            return new List<ParsedMessage>();
        }
    }

    private string ExtractJsonArray(string text)
    {
        // Try to find JSON array in the response
        var match = Regex.Match(text, @"\[[\s\S]*\]", RegexOptions.Multiline);
        if (match.Success)
        {
            return match.Value;
        }

        // If the entire text is already a JSON array
        if (text.TrimStart().StartsWith('['))
        {
            return text.Trim();
        }

        return string.Empty;
    }
}

public class AIConfiguration
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4";
    public string SystemPrompt { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
}
