using System.Net.Http.Json;
using Application.Dtos;
using Application.Dtos.JustJoinIt;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.HttpClients;

public class JustJoinItJobBoardHttpClient : BaseJobBoardHttpClient, IJustJoinItJobBoardHttpClient
{

    public JustJoinItJobBoardHttpClient(HttpClient httpClient, IConfiguration config) : base(httpClient)
    {
        httpClient.BaseAddress = new(config["JustJoinIt:Url"]!);
        httpClient.DefaultRequestHeaders.Add("Version", "2");
    }

    public async Task<IEnumerable<JobAdCreateDto>> GetJobsAsync()
    {
        HttpContent content = await GetJobsAsync("1");

        var test = await content.ReadAsStringAsync();

        var  justJoinItResponse = await content.ReadFromJsonAsync<JustJoinItResponse>();

        if (justJoinItResponse == null)
        {
            throw new Exception("JustJoinIt response empty.");
        }

        return justJoinItResponse.GenerateJobAdCreateDtos();

    }
}
