namespace Domain.Entities;
public record Company
{
    public Company() { }

    public int Id { get; set; }
    public required List<CompanyName> Names { get; set; }
}
