using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Dtos;

public record JobAdDetailsDto(
    int JobAdId,
    string ExternalId,
    string Title,
    OfferStatus OfferStatus,
    WorkplaceType[] WorkplaceType,
    ExperienceLevel[] ExperienceLevel,
    int? CompanyId,
    string? CompanyName,
    JobSite JobSite,
    string Slug,
    DateTimeOffset? ExpiredAt,
    DateTimeOffset? PublishedAt,
    IReadOnlyCollection<string> Skills,
    IReadOnlyCollection<SalaryDto> Salaries,
    IReadOnlyCollection<DescriptionDto> Descriptions);

