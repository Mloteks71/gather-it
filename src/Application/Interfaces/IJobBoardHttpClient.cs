using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces;
public interface IJobBoardHttpClient
{
    Task<IEnumerable<JobAdCreateDto>> GetJobsAsync();
}
