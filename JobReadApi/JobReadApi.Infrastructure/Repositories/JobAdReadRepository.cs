using JobReadApi.Application.Dtos;
using JobReadApi.Application.Enums;
using JobReadApi.Application.Interfaces;
using JobReadApi.Infrastructure.Data;
using JobReadApi.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace JobReadApi.Infrastructure.Repositories;

public class JobAdReadRepository(ReadApiDbContext dbContext) : IJobAdReadRepository
{
    public async Task<PagedResultDto<JobAdListItemDto>> GetPagedAsync(int page, int pageSize, OfferStatus? status, JobSite? site, CancellationToken cancellationToken)
    {
        var query = dbContext.JobAd
            .AsNoTracking()
            .Include(x => x.Company)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.OfferStatus == status.Value);
        }

        if (site.HasValue)
        {
            query = query.Where(x => x.JobSite == site.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(JobAdMapper.ToListItemDtoExpression)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<JobAdListItemDto>(items, page, pageSize, totalCount);
    }

    public async Task<JobAdDetailsDto?> GetByIdAsync(int jobAdId, CancellationToken cancellationToken)
    {
        return await dbContext.JobAd
            .AsNoTracking()
            .Where(x => x.JobAdId == jobAdId)
            .Select(JobAdMapper.ToDetailsDtoExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

