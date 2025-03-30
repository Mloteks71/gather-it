using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IJobAdRepository
{
    public Task InsertJobAds(IEnumerable<JobAdCreateDto> jobAds);
    public IEnumerable<JobAd> GetJobAds(IEnumerable<string> companyNames);
}
