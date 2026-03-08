using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Models.Dtos;

namespace Application.Services;

public class Comulator : IComulator
{
    private readonly IJustJoinItHttpClient _justJoinItHttpClient;
    private readonly ITheProtocolItHttpClient _theProtocolItHttpClient;
    private readonly IResponseMapper _responseMapper;

    public Comulator(
        IJustJoinItHttpClient justJoinItHttpClient,
        ITheProtocolItHttpClient theProtocolItHttpClient,
        IResponseMapper responseMapper)
    {
        _justJoinItHttpClient = justJoinItHttpClient;
        _theProtocolItHttpClient = theProtocolItHttpClient;
        _responseMapper = responseMapper;
    }

    public async Task<IEnumerable<CommonJobAdDto>> Comulate()
    {
        var justJoinItResponseTask = _justJoinItHttpClient.GetJobsAsync();
        var theProtocolItResponseTask = _theProtocolItHttpClient.GetJobsAsync();

        await Task.WhenAll(justJoinItResponseTask, theProtocolItResponseTask);

        var justJoinItJobs = _responseMapper.MapJustJoinItResponse(justJoinItResponseTask.Result);
        var theProtocolItJobs = _responseMapper.MapTheProtocolItResponse(theProtocolItResponseTask.Result);

        return justJoinItJobs.Concat(theProtocolItJobs);
    }
}
