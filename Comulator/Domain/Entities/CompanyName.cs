namespace Domain.Entities;
public record CompanyName
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CompanyName() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public CompanyName(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public int JobAdId { get; set; }
    public JobAd JobAd { get; set; } = null!;
}
