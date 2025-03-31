using Application.Dtos;
using Application.Interfaces;

namespace Infrastructure.Services;
public class Comulator(IJustJoinItJobBoardHttpClient justJoinItHttpClient) : IComulator
{
    private readonly IJustJoinItJobBoardHttpClient _justJoinItHttpClient = justJoinItHttpClient;

    public async Task<IEnumerable<JobAdCreateDto>> Comulate()
    {
        var justJoinItJobs = await _justJoinItHttpClient.GetJobsAsync();

        return justJoinItJobs;
    }
}