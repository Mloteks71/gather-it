using Domain.Enums;

namespace Domain.Entities;
public record JobAd
{
    public JobAd() { }

    public int Id { get; set; }
    public required string Name { get; set; } = null!;
    public required RemoteType RemoteType { get; set; }
    public required int? RemotePercent { get; set; }
    public required ICollection<City> Cities { get; set; } = [];
    public required ICollection<Salary> Salaries { get; set; } = [];
    public required int CompanyNameId { get; set; }
    public required CompanyName CompanyName { get; set; } = null!;
    public required string Slug { get; set; } = null!;
    public required Site Site { get; set; }
    public Description? Description { get; set; }
    //public JustJoinItCategory JobCategoryId { get; set; }
}
