using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.HttpClients;

namespace Infrastructure.Services;
public class Comulator : IComulator
{
    private readonly IJustJoinItHttpClient _justJoinItHttpClient;
    private readonly ITheProtocolItHttpClient _theProtocolItHttpClient;
    private readonly ISolidJobsHttpClient _solidJobsHttpClient;

    public Comulator(IJustJoinItHttpClient justJoinItHttpClient, ITheProtocolItHttpClient theProtocolItHttpClient, ISolidJobsHttpClient solidJobsHttpClient)
    {
        _justJoinItHttpClient = justJoinItHttpClient;
        _theProtocolItHttpClient = theProtocolItHttpClient;
        _solidJobsHttpClient = solidJobsHttpClient;
    }

    public async Task<IEnumerable<JobAdCreateDto>> Comulate()
    {
        var justJoinItJobs = _justJoinItHttpClient.GetJobsAsync();
        var solidJobsJobs = _solidJobsHttpClient.GetJobsAsync();
        var theProtocolItJobs = _theProtocolItHttpClient.GetJobsAsync();

        await Task.WhenAll(justJoinItJobs, solidJobsJobs);

        var result = new List<JobAdCreateDto>();
        result.AddRange(justJoinItJobs.Result.Chunk(50).First());
        result.AddRange(solidJobsJobs.Result.Chunk(50).First());
        result.AddRange(theProtocolItJobs.Result.Chunk(50).First());

        return result;
    }
}
