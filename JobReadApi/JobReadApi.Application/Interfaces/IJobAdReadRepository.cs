using JobReadApi.Application.Dtos;
using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Interfaces;

public interface IJobAdReadRepository
{
    Task<PagedResultDto<JobAdListItemDto>> GetPagedAsync(int page, int pageSize, OfferStatus? status, JobSite? site, CancellationToken cancellationToken);
    Task<JobAdDetailsDto?> GetByIdAsync(int jobAdId, CancellationToken cancellationToken);
}

