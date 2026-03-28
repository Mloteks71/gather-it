using Domain.Enums;

namespace Application.Models.Dtos;

public class CommonJobAdDto
{
    public required string Id { get; init; }
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string CompanyName { get; init; }
    public required Site SourceSite { get; init; }
    public List<string>? Skills { get; init; }
    public List<string>? WorkplaceTypes { get; init; }
    public List<string>? ExperienceLevels { get; init; }
    public List<string>? Locations { get; init; }
    public List<SalaryRangeDto>? Salaries { get; init; }
    public DateTime? PublishedAt { get; init; }
    public string? LogoUrl { get; init; }
}
