using System.Diagnostics;
using System.Net.Http.Json;
using Application.Dtos;
using Application.Dtos.SolidJobs;
using Application.Interfaces.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.HttpClients;

public class SolidJobsHttpClient : BaseJobBoardHttpClient, ISolidJobsHttpClient
{
    private readonly Uri _uri;
    public SolidJobsHttpClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<SolidJobsHttpClient> logger) : base(httpClient, logger)
    {
        _uri = new Uri(config["SolidJobs:Url"]!);
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var content = await GetJobsAsync(_uri);
        var solidJobsResponse = await content.ReadFromJsonAsync<IEnumerable<SolidJobsResponse>>();

        if (solidJobsResponse is null)
            throw new InvalidOperationException("SolidJobs API returned null response. The API may be unavailable or the response format has changed.");

        var responseList = solidJobsResponse.ToList();
        if (responseList.Count == 0)
        {
            Logger.LogWarning("SolidJobs returned no job postings.");
            return new List<JobAdCreateDto>();
        }

        var result = SolidJobsResponse.GenerateJobAdCreateDtos(responseList).ToList();

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        Logger.LogInformation("Fetched {JobAdsCount} job ads from SolidJobs in {ElapsedMilliseconds} ms", result.Count, elapsedMilliseconds);

        return result;
    }
}
