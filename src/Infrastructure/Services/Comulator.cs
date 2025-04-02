using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.HttpClients;

namespace Infrastructure.Services;
public class Comulator : IComulator {
    private readonly IJustJoinItJobBoardHttpClient _justJoinItHttpClient;
    private readonly ISolidJobsHttpClient _solidJobsHttpClient;

    public Comulator(IJustJoinItJobBoardHttpClient justJoinItHttpClient, ISolidJobsHttpClient solidJobsHttpClient) {
        _justJoinItHttpClient = justJoinItHttpClient;
        _solidJobsHttpClient = solidJobsHttpClient;
    }

    public async Task<IEnumerable<JobAdCreateDto>> Comulate() {
        var justJoinItJobs = _justJoinItHttpClient.GetJobsAsync();
        var solidJobsJobs = _solidJobsHttpClient.GetJobsAsync();

        await Task.WhenAll(justJoinItJobs, solidJobsJobs);

        var result = new List<JobAdCreateDto>();
        result.AddRange(justJoinItJobs.Result);
        result.AddRange(solidJobsJobs.Result);

        return result;
    }
}