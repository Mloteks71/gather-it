using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Models.Dtos;

namespace Application.Services;

public class Comulator : IComulator
{
    private readonly IJustJoinItHttpClient _justJoinItHttpClient;
    private readonly ITheProtocolItHttpClient _theProtocolItHttpClient;

    public Comulator(
        IJustJoinItHttpClient justJoinItHttpClient,
        ITheProtocolItHttpClient theProtocolItHttpClient
    )
    {
        _justJoinItHttpClient = justJoinItHttpClient;
        _theProtocolItHttpClient = theProtocolItHttpClient;
    }

    public async Task<IEnumerable<CommonJobAdDto>> Comulate()
    {
        var justJoinItJobs = _justJoinItHttpClient.GetJobsAsync();
        var theProtocolItJobs = _theProtocolItHttpClient.GetJobsAsync();

        var results = await Task.WhenAll(justJoinItJobs, theProtocolItJobs);

        return results.SelectMany(x => x);
    }
}
