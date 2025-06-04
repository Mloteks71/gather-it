using Domain.Enums;

namespace Domain.Entities;
public record Salary
{
    public Salary() { }
    public Salary(ContractType contractType, int salaryMin, int salaryMax)
    {
        ContractType = contractType;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
    }

    public int Id { get; set; }
    public ContractType ContractType { get; set; }
    public int SalaryMin { get; set; }
    public int SalaryMax { get; set; }
    public int JobAdId { get; set; }
    public JobAd? JobAd { get; set; }
}
