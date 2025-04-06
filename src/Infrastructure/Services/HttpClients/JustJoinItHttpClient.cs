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
        httpClient.DefaultRequestHeaders.Add("Version", "2");
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
            throw new Exception("JustJoinIt response empty.");

        if (justJoinItResponse.Jobs.Count == 0)
            return result;

        _totalPages = justJoinItResponse.MetaData.TotalPages;

        result.AddRange(justJoinItResponse.GenerateJobAdCreateDtos());

        var pagesToFetch = Enumerable
            .Range(2, _totalPages)
            .ToDictionary(x => x, x => GetJobsAsync(new Uri($"{_uri}{x}")));

        await Task.WhenAll(pagesToFetch.Values);

        var pagesToMap = Enumerable
            .Range(2, _totalPages)
            .ToDictionary(x => x, x => pagesToFetch[x].Result.ReadFromJsonAsync<JustJoinItResponse>());

        await Task.WhenAll(pagesToMap.Values);

        var dataToAdd = pagesToMap
            .Where(x => x.Value.Result is not null)
            .SelectMany(x => x.Value.Result!.GenerateJobAdCreateDtos());

        result.AddRange(dataToAdd);

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        Logger.LogInformation("Fetched {JobAdsCount} job ads from JustJoinIt in {ElapsedMilliseconds} ms", result.Count, elapsedMilliseconds);

        return result;
    }
}
