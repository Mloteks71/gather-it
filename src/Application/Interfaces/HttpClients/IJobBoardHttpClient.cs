using Application.Dtos;

namespace Application.Interfaces.HttpClients;
public interface IJobBoardHttpClient
{
    Task<IEnumerable<JobAdCreateDto>> GetJobsAsync();
}
