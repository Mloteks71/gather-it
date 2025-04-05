namespace Infrastructure.Services;
public abstract class BaseJobBoardHttpClient
{
    private readonly HttpClient _httpClient;

    public BaseJobBoardHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<HttpContent> GetJobsAsync(Uri uri, bool usePost = false, HttpContent? requestContent = null)
    {
        HttpResponseMessage response = usePost ? await _httpClient.PostAsync(uri, requestContent) : await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.StatusCode + "  " + response.Content);
        }

        return response.Content;
    }
}
