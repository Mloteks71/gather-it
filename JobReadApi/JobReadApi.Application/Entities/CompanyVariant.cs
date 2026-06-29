namespace JobReadApi.Application.Entities;

public class CompanyVariant
{
    public int CompanyVariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CompanyId { get; set; }

    public Company Company { get; set; } = null!;
}

