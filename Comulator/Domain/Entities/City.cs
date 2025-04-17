namespace Domain.Entities;
public record City
{
    public City(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public List<JobAd>? JobAds { get; set; }
}
