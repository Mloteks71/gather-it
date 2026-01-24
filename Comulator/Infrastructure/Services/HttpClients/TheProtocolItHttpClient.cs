using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using Application.Dtos;
using Application.Dtos.TheProtocolIt;
using Application.Interfaces.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.HttpClients;

public class TheProtocolItHttpClient : BaseJobBoardHttpClient, ITheProtocolItHttpClient
{
    private readonly Uri _uri;
    private int _totalPages = 1;
    public TheProtocolItHttpClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<TheProtocolItResponse> logger) : base(httpClient, logger)
    {
        _uri = new Uri(config["TheProtocolIt:Url"]!);
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = new List<JobAdCreateDto>();
        var firstPage = new Uri($"{_uri}{1}");
        var requestContent = new StringContent("", Encoding.UTF8, "application/json");
        var responseContent = await GetJobsAsync(firstPage, true, requestContent);
        var theProtocolItResponse = await responseContent.ReadFromJsonAsync<TheProtocolItResponse>();

        if (theProtocolItResponse is null)
            throw new InvalidOperationException("TheProtocolIt API returned null response. The API may be unavailable or the response format has changed.");

        if (theProtocolItResponse.Offers is null || theProtocolItResponse.Offers.Count == 0)
        {
            Logger.LogWarning("TheProtocolIt returned no job postings.");
            return result;
        }

        _totalPages = theProtocolItResponse.Page.Count;

        result.AddRange(theProtocolItResponse.GenerateJobAdCreateDtos());

        var pagesToFetch = Enumerable
            .Range(2, _totalPages)
            .Select(async pageNumber =>
            {
                await Task.Delay(300);
                var content = await GetJobsAsync(new Uri($"{_uri}{pageNumber}"), true, requestContent);
                return await content.ReadFromJsonAsync<TheProtocolItResponse>();
            });

        var responses = await Task.WhenAll(pagesToFetch);

        var dataToAdd = responses
            .Where(x => x is not null)
            .SelectMany(x => x!.GenerateJobAdCreateDtos());

        result.AddRange(dataToAdd);

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        Logger.LogInformation("Fetched {JobAdsCount} job ads from TheProtocolIt in {ElapsedMilliseconds} ms", result.Count, elapsedMilliseconds);

        return result;
    }
}
