using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Entities;

public class Salary
{
    public int SalaryId { get; set; }
    public ContractType ContractType { get; set; }
    public float? SalaryMin { get; set; }
    public float? SalaryMax { get; set; }
    public int JobAdId { get; set; }

    public JobAd JobAd { get; set; } = null!;
}

