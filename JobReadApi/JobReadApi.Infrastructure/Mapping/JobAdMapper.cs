using System.Linq.Expressions;
using JobReadApi.Application.Dtos;
using JobReadApi.Application.Entities;

namespace JobReadApi.Infrastructure.Mapping;

public static class JobAdMapper
{
    public static Expression<Func<JobAd, JobAdListItemDto>> ToListItemDtoExpression => x => new JobAdListItemDto(
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
        x.PublishedAt);

    public static Expression<Func<JobAd, JobAdDetailsDto>> ToDetailsDtoExpression => x => new JobAdDetailsDto(
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
            .ToList());
}
