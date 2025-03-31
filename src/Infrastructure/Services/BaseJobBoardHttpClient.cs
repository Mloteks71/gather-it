namespace Infrastructure.Services;
public abstract class BaseJobBoardHttpClient {
    private readonly HttpClient _httpClient;
    public BaseJobBoardHttpClient(HttpClient httpClient) {
        _httpClient = httpClient;
    }
    protected async Task<HttpContent> GetJobsAsync(Uri uri)
    {
        var response = await _httpClient.GetAsync(uri);
    
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.StatusCode + "  " + response.Content);
        }
    
        return response.Content;
    }
}
