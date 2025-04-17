namespace Domain.Entities;
public record CompanyName
{
    public CompanyName(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
}
