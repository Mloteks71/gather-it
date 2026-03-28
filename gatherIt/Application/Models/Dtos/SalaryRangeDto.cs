namespace Application.Models.Dtos;

public class SalaryRangeDto
{
    public decimal? From { get; init; }
    public decimal? To { get; init; }
    public required string Currency { get; init; }
    public string? ContractType { get; init; }
}
