using Application.Dtos;
using Application.Interfaces;

namespace Infrastructure.Services;

public class JobAdService : IJobAdService {
    public IEnumerable<JobAdCreateDto> RemoveJobAds(IEnumerable<JobAdCreateDto> originalJobAds,
        Func<JobAdCreateDto, bool> predicate) => originalJobAds.Where(predicate);
}