using Application.Models.Responses;

namespace Application.Interfaces.HttpClients;

public interface IJustJoinItHttpClient
{
    Task<JustJoinItResponse> GetJobsAsync();
}
