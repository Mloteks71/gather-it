using Microsoft.Extensions.Logging;

namespace Application.Services.HttpClients;

public abstract class BaseJobBoardHttpClient
{
    private readonly HttpClient _httpClient;
    protected readonly ILogger Logger;

    public BaseJobBoardHttpClient(HttpClient httpClient, ILogger logger)
    {
        Logger = logger;
        _httpClient = httpClient;
    }

    protected async Task<HttpContent> GetJobsAsync(
        Uri uri,
        bool usePost = false,
        HttpContent? requestContent = null
    )
    {
        HttpResponseMessage response = usePost
            ? await _httpClient.PostAsync(uri, requestContent)
            : await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"HTTP request failed with status {response.StatusCode}: {content}",
                null,
                response.StatusCode
            );
        }

        return response.Content;
    }
}
