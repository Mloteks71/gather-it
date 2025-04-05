using Application.Dtos;
using Application.Dtos.TheProtocolIt;
using Application.Interfaces.HttpClients;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;

namespace Infrastructure.Services.HttpClients;

public class TheProtocolItHttpClient : BaseJobBoardHttpClient, ITheProtocolItHttpClient
{
    private readonly Uri _uri;
    private int _totalPages = 1;
    private Dictionary<string, string?> _headers;
    public TheProtocolItHttpClient(HttpClient httpClient, IConfiguration config) : base(httpClient)
    {
        _uri = new Uri(config["TheProtocolIt:Url"]!);
        var headers = config.GetSection("TheProtocolIt:HttpHeaders")
                            .GetChildren()
                            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var header in headers)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
        _headers = headers;
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync()
    {
        var result = new List<JobAdCreateDto>();
        var firstPage = new Uri($"{_uri}{1}");
        var requestContent = new StringContent("", Encoding.UTF8, "application/json");
        var responseContent = await GetJobsAsync(firstPage, true, requestContent);
        var theProtocolItResponse = await responseContent.ReadFromJsonAsync<TheProtocolItResponse>();

        if (theProtocolItResponse is null)
            throw new Exception("JustJoinIt response empty.");

        if (theProtocolItResponse.Offers.Count == 0)
            return result;

        _totalPages = theProtocolItResponse.Page.Count;

        result.AddRange(theProtocolItResponse.GenerateJobAdCreateDtos());

        Dictionary<int, TheProtocolItResponse> pagesToMap = [];
        for (int x = 2; x <= _totalPages + 1; x++)
        {
            await Task.Delay(200);
            var content = await GetJobsAsync(new Uri($"{_uri}{x}"), true, requestContent);
            pagesToMap[x] = (await content.ReadFromJsonAsync<TheProtocolItResponse>())!;
        }

        var dataToAdd = pagesToMap
            .Where(x => x.Value is not null)
            .SelectMany(x => x.Value!.GenerateJobAdCreateDtos());

        result.AddRange(dataToAdd);

        return result;
    }
}
