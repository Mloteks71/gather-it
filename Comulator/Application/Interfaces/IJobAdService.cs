using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IJobAdService
{
    public IEnumerable<JobAdCreateDto> RemoveJobAds(
        IEnumerable<JobAdCreateDto> originalJobAds,
        Func<JobAdCreateDto, bool> predicate);
}
