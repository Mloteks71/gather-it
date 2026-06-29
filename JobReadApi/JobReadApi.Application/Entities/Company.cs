namespace JobReadApi.Application.Entities;

public class Company
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CompanyVariant> Variants { get; set; } = [];
    public ICollection<JobAd> JobAds { get; set; } = [];
}

