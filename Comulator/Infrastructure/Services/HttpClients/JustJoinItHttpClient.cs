using System.Diagnostics;
using System.Net.Http.Json;
using Application.Dtos;
using Application.Dtos.JustJoinIt;
using Application.Interfaces.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.HttpClients;

public class JustJoinItHttpClient : BaseJobBoardHttpClient, IJustJoinItHttpClient
{
    private readonly Uri _uri;
    private int _totalPages = 1;
    public JustJoinItHttpClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<JustJoinItHttpClient> logger) : base(httpClient, logger)
    {
        _uri = new Uri(config["JustJoinIt:Url"]!);
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = new List<JobAdCreateDto>();
        var firstPage = new Uri($"{_uri}{1}");
        var content = await GetJobsAsync(firstPage);
        var justJoinItResponse = await content.ReadFromJsonAsync<JustJoinItResponse>();

        if (justJoinItResponse is null)
            throw new InvalidOperationException("JustJoinIt API returned null response. The API may be unavailable or the response format has changed.");

        if (justJoinItResponse.Jobs is null || justJoinItResponse.Jobs.Count == 0)
        {
            Logger.LogWarning("JustJoinIt returned no job postings.");
            return result;
        }

        _totalPages = justJoinItResponse.MetaData.TotalPages;

        result.AddRange(justJoinItResponse.GenerateJobAdCreateDtos());

        var pagesToFetch = Enumerable
            .Range(2, _totalPages - 1)
            .Select(x => GetJobsAsync(new Uri($"{_uri}{x}")))
            .ToList();

        var pageContents = await Task.WhenAll(pagesToFetch);

        var deserializationTasks = pageContents
            .Select(content => content.ReadFromJsonAsync<JustJoinItResponse>());

        var responses = await Task.WhenAll(deserializationTasks);

        var dataToAdd = responses
            .Where(response => response is not null)
            .SelectMany(response => response!.GenerateJobAdCreateDtos());

        result.AddRange(dataToAdd);

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        Logger.LogInformation("Fetched {JobAdsCount} job ads from JustJoinIt in {ElapsedMilliseconds} ms", result.Count, elapsedMilliseconds);

        return result;
    }
}
