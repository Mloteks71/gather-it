using System.Net.Http.Json;
using Application.Dtos;
using Application.Dtos.SolidJobs;
using Application.Interfaces.HttpClients;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.HttpClients;

public class SolidJobsHttpClient : BaseJobBoardHttpClient, ISolidJobsHttpClient {
    private readonly Uri _uri;
    public SolidJobsHttpClient(HttpClient httpClient, IConfiguration config) : base(httpClient) {
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.solidjobs.jobofferlist+json, application/json, text/plain, */*");
        _uri = new Uri(config["SolidJobs:Url"]!);
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync() {
        var content = await GetJobsAsync(_uri);
        var solidJobsResponse = await content.ReadFromJsonAsync<IEnumerable<SolidJobsResponse>>();
        
        if (solidJobsResponse is null)
            throw new Exception("SolidJobs response empty.");
        
        var result = SolidJobsResponse.GenerateJobAdCreateDtos(solidJobsResponse);

        return result;
    }
}