using Application.Models.Dtos;

namespace Application.Interfaces.HttpClients;

public interface IJobBoardHttpClient
{
    Task<IEnumerable<JobAdCreateDto>> GetJobsAsync();
}
