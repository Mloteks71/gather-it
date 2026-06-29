using Microsoft.EntityFrameworkCore;
using JobReadApi.Application.Dtos;
using JobReadApi.Application.Enums;
using JobReadApi.Application.Interfaces;
using JobReadApi.Infrastructure.Data;

namespace JobReadApi.Infrastructure.Repositories;

public class JobAdReadRepository(ReadApiDbContext dbContext) : IJobAdReadRepository
{
    public async Task<PagedResultDto<JobAdListItemDto>> GetPagedAsync(int page, int pageSize, OfferStatus? status, JobSite? site, CancellationToken cancellationToken)
    {
        var query = dbContext.JobAds
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
            .Select(x => new JobAdListItemDto(
                x.JobAdId,
                x.ExternalId,
                x.Title,
                x.OfferStatus,
                x.WorkplaceType,
                x.ExperienceLevel,
                x.Company != null ? x.Company.Name : null,
                x.JobSite,
                x.Slug,
                x.ExpiredAt,
                x.PublishedAt))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<JobAdListItemDto>(items, page, pageSize, totalCount);
    }

    public async Task<JobAdDetailsDto?> GetByIdAsync(int jobAdId, CancellationToken cancellationToken)
    {
        return await dbContext.JobAds
            .AsNoTracking()
            .Where(x => x.JobAdId == jobAdId)
            .Select(x => new JobAdDetailsDto(
                x.JobAdId,
                x.ExternalId,
                x.Title,
                x.OfferStatus,
                x.WorkplaceType,
                x.ExperienceLevel,
                x.CompanyId,
                x.Company != null ? x.Company.Name : null,
                x.JobSite,
                x.Slug,
                x.ExpiredAt,
                x.PublishedAt,
                x.JobAdSkills.Select(js => js.Skill.Name).Distinct().ToList(),
                x.Salaries
                    .Select(s => new SalaryDto(s.SalaryId, s.ContractType, s.SalaryMin, s.SalaryMax))
                    .ToList(),
                x.Descriptions
                    .Select(d => new DescriptionDto(d.DescriptionId, d.DescriptionText, d.Requirements, d.Benefits, d.Workstyle, d.AboutProject))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

