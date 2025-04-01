using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces;

public interface IJobAdService {
    public IEnumerable<JobAdCreateDto> RemoveDuplicateJobAds(
        IEnumerable<JobAdCreateDto> jobAdsToAdd,
        Func<JobAdCreateDto, bool> predicate);
}