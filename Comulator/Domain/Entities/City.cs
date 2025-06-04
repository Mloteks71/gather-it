namespace Domain.Entities;
public record City
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public City() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public City(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public List<JobAd>? JobAds { get; set; }
}
