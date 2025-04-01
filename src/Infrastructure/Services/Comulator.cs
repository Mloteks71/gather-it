using Application.Dtos;
using Application.Interfaces;

namespace Infrastructure.Services;
public class Comulator : IComulator {
    private readonly IJustJoinItJobBoardHttpClient _justJoinItHttpClient;

    public Comulator(IJustJoinItJobBoardHttpClient justJoinItHttpClient) {
        _justJoinItHttpClient = justJoinItHttpClient;
    }

    public async Task<IEnumerable<JobAdCreateDto>> Comulate()
    {
        var justJoinItJobs = await _justJoinItHttpClient.GetJobsAsync();

        return justJoinItJobs;
    }
}