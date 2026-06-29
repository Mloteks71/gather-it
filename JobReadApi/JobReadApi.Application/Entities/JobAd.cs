using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Entities;

public class JobAd
{
    public int JobAdId { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public OfferStatus OfferStatus { get; set; }
    public WorkplaceType[] WorkplaceType { get; set; } = [];
    public ExperienceLevel[] ExperienceLevel { get; set; } = [];
    public int? CompanyId { get; set; }
    public JobSite JobSite { get; set; }
    public string Slug { get; set; } = string.Empty;
    public DateTimeOffset? ExpiredAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }

    public Company? Company { get; set; }
    public ICollection<Description> Descriptions { get; set; } = [];
    public ICollection<Salary> Salaries { get; set; } = [];
    public ICollection<JobAdSkill> JobAdSkills { get; set; } = [];
}

