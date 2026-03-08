using Application.Models.Responses;

namespace Application.Interfaces.HttpClients;

public interface ITheProtocolItHttpClient
{
    Task<TheProtocolItResponse> GetJobsAsync();
}
