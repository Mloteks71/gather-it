using Application.Interfaces;
using Application.Models.Dtos;

namespace Application.Services;

public class JobAdService : IJobAdService
{
    public IEnumerable<JobAdCreateDto> RemoveJobAds(IEnumerable<JobAdCreateDto> originalJobAds,
        Func<JobAdCreateDto, bool> predicate) => originalJobAds.Where(predicate);
}
