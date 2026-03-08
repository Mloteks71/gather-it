using System.Diagnostics;
using System.Net.Http.Json;
using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services.HttpClients;

public class JustJoinItHttpClient : BaseJobBoardHttpClient, IJustJoinItHttpClient
{
    private readonly Uri _uri;
    private int _totalPages = 1;
    public JustJoinItHttpClient(
        HttpClient httpClient,
        IConfigurationService config,
        ILogger<JustJoinItHttpClient> logger) : base(httpClient, logger)
    {
        _uri = new Uri(config.JustJoinItUrl);
    }

    public async Task<JustJoinItResponse> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var firstPage = new Uri($"{_uri}{1}");
        var content = await base.GetJobsAsync(firstPage);
        var justJoinItResponse = await content.ReadFromJsonAsync<JustJoinItResponse>();

        if (justJoinItResponse is null)
            throw new InvalidOperationException("JustJoinIt API returned null response. The API may be unavailable or the response format has changed.");

        if (justJoinItResponse.Data is null || justJoinItResponse.Data.Count == 0)
        {
            Logger.LogWarning("JustJoinIt returned no job postings.");
            return justJoinItResponse;
        }

        _totalPages = justJoinItResponse.Meta.TotalPages;

        if (_totalPages > 1)
        {
            var pagesToFetch = Enumerable
                .Range(2, _totalPages - 1)
                .Select(x => base.GetJobsAsync(new Uri($"{_uri}{x}")));

            var pageContents = await Task.WhenAll(pagesToFetch);

            var deserializationTasks = pageContents
                .Select(content => content.ReadFromJsonAsync<JustJoinItResponse>());

            var responses = await Task.WhenAll(deserializationTasks);

            var additionalJobAds = responses
                .Where(response => response is not null)
                .SelectMany(response => response!.Data);

            justJoinItResponse.Data.AddRange(additionalJobAds);
        }

        stopwatch.Stop();
        Logger.LogInformation("Fetched {JobAdsCount} job ads from JustJoinIt in {ElapsedMilliseconds} ms", 
            justJoinItResponse.Data.Count, stopwatch.ElapsedMilliseconds);

        return justJoinItResponse;
    }
}
