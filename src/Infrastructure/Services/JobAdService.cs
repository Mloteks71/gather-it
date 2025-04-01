using Application.Dtos;
using Application.Interfaces;

namespace Infrastructure.Services;

public class JobAdService : IJobAdService {
    public IEnumerable<JobAdCreateDto> RemoveDuplicateJobAds(IEnumerable<JobAdCreateDto> jobAdsToAdd,
        Func<JobAdCreateDto, bool> predicate) => jobAdsToAdd.Where(predicate);
}