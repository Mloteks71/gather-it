using System.Diagnostics;
using System.Net.Http.Json;
using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Models.Dtos;
using Application.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services.HttpClients;

public class JustJoinItHttpClient : BaseJobBoardHttpClient, IJustJoinItHttpClient
{
    private readonly Uri _uri;
    private readonly IResponseMapper _responseMapper;

    public JustJoinItHttpClient(
        HttpClient httpClient,
        ILogger<JustJoinItHttpClient> logger,
        IConfigurationService config,
        IResponseMapper responseMapper
    )
        : base(httpClient, logger)
    {
        _uri = new Uri(config.JustJoinItUrl);
        _responseMapper = responseMapper;
    }

    public async Task<IEnumerable<CommonJobAdDto>> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var firstPage = new Uri($"{_uri}{1}");

        var content = await GetJobsAsync(firstPage);

        var justJoinItResponse = await content.ReadFromJsonAsync<JustJoinItResponse>();
        if (justJoinItResponse is null)
            throw new InvalidOperationException(
                "JustJoinIt API returned null response. The API may be unavailable or the response format has changed."
            );

        if (justJoinItResponse.Data.Count == 0)
        {
            Logger.LogWarning("JustJoinIt returned no job postings.");
            return [];
        }

        var totalPages = justJoinItResponse.Meta.TotalPages;

        if (totalPages > 1)
        {
            var pagesToFetch = Enumerable
                .Range(2, totalPages - 1)
                .Select(x => GetJobsAsync(new Uri($"{_uri}{x}")));

            var pageContents = await Task.WhenAll(pagesToFetch);

            var deserializationTasks = pageContents.Select(content =>
                content.ReadFromJsonAsync<JustJoinItResponse>()
            );

            var responses = await Task.WhenAll(deserializationTasks);

            var additionalJobAds = responses
                .Where(response => response is not null)
                .SelectMany(response => response!.Data);

            justJoinItResponse.Data.AddRange(additionalJobAds);
        }

        Logger.LogInformation(
            "Fetched {JobAdsCount} job ads from JustJoinIt in {ElapsedMilliseconds} ms",
            // temp value
            3120812,
            stopwatch.ElapsedMilliseconds
        );

        return _responseMapper.MapJustJoinItResponse(justJoinItResponse);
    }
}
