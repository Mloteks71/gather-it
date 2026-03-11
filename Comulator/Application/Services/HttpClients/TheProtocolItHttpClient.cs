using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services.HttpClients;

public class TheProtocolItHttpClient : BaseJobBoardHttpClient, ITheProtocolItHttpClient
{
    private readonly Uri _uri;
    private int _totalPages = 1;
    public TheProtocolItHttpClient(
        HttpClient httpClient,
        IConfigurationService config,
        ILogger<TheProtocolItResponse> logger) : base(httpClient, logger)
    {
        _uri = new Uri(config.TheProtocolItUrl);
    }

    public async Task<TheProtocolItResponse> GetJobsAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var firstPage = new Uri($"{_uri}{1}");
        var requestContent = new StringContent("", Encoding.UTF8, "application/json");
        var responseContent = await base.GetJobsAsync(firstPage, true, requestContent);
        var theProtocolItResponse = await responseContent.ReadFromJsonAsync<TheProtocolItResponse>();

        if (theProtocolItResponse is null)
            throw new InvalidOperationException("TheProtocolIt API returned null response. The API may be unavailable or the response format has changed.");

        if (theProtocolItResponse.Offers is null || theProtocolItResponse.Offers.Count == 0)
        {
            Logger.LogWarning("TheProtocolIt returned no job postings.");
            return theProtocolItResponse;
        }

        _totalPages = theProtocolItResponse.Page.Count;

        if (_totalPages > 1)
        {
            for (int pageNumber = 2; pageNumber <= _totalPages; pageNumber++)
            {
                await Task.Delay(300);
                var content = await base.GetJobsAsync(new Uri($"{_uri}{pageNumber}"), true, requestContent);
                var response = await content.ReadFromJsonAsync<TheProtocolItResponse>();

                if (response?.Offers is not null)
                {
                    theProtocolItResponse.Offers.AddRange(response.Offers);
                }
            }
        }

        stopwatch.Stop();
        Logger.LogInformation(
            "Fetched {JobAdsCount} job ads from TheProtocolIt in {ElapsedMilliseconds} ms",
            theProtocolItResponse.Offers.Count,
            stopwatch.ElapsedMilliseconds);

        return theProtocolItResponse;
    }
}
