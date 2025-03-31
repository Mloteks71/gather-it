using System.Net.Http.Json;
using Application.Dtos;
using Application.Dtos.JustJoinIt;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.HttpClients;

public class JustJoinItJobBoardHttpClient : BaseJobBoardHttpClient, IJustJoinItJobBoardHttpClient {
    private readonly Uri _uri;
    private int _totalPages = 1;
    public JustJoinItJobBoardHttpClient(HttpClient httpClient, IConfiguration config) : base(httpClient)
    {
        _uri = new Uri(config["JustJoinIt:Url"]!);
        httpClient.DefaultRequestHeaders.Add("Version", "2");
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync() {
        var result = new List<JobAdCreateDto>();
        
        for (var i = 1 ; i <= _totalPages; i++) {
            var currentPageUri = new Uri($"{_uri}{i}");
            var content = await GetJobsAsync(currentPageUri);
            var justJoinItResponse = await content.ReadFromJsonAsync<JustJoinItResponse>();

            if (justJoinItResponse == null)
                throw new Exception("JustJoinIt response empty.");
            
            if (justJoinItResponse.Jobs.Count == 0)
                break;
            
            _totalPages = justJoinItResponse.MetaData.TotalPages;
            
            result.AddRange(justJoinItResponse.GenerateJobAdCreateDtos());
        }

        return result;
    }
}
