using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Dtos;

public record JobAdListItemDto(
    int JobAdId,
    string ExternalId,
    string Title,
    OfferStatus OfferStatus,
    WorkplaceType[] WorkplaceType,
    ExperienceLevel[] ExperienceLevel,
    string? CompanyName,
    JobSite JobSite,
    string Slug,
    DateTimeOffset? ExpiredAt,
    DateTimeOffset? PublishedAt);

