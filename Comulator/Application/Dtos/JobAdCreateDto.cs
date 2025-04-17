using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos;

public class JobAdCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public RemoteType RemoteType { get; set; }
    public int? RemotePercent { get; set; }
    public IEnumerable<City> Cities { get; set; }
    public IEnumerable<Salary> Salaries { get; set; }
    public int CompanyNameId { get; set; }
    public CompanyName CompanyName { get; set; }
    public string Slug { get; set; }
    public Site Site { get; set; }
    
    public JobAdCreateDto(
        string name,
        string description,
        RemoteType remoteType,
        int? remotePercent,
        IEnumerable<City> cities,
        IEnumerable<Salary> salaries,
        int companyNameId,
        CompanyName companyName,
        string slug,
        Site site)
    {
        Name = name;
        Description = description;
        RemoteType = remoteType;
        RemotePercent = remotePercent;
        Cities = cities;
        Salaries = salaries;
        CompanyNameId = companyNameId;
        CompanyName = companyName;
        Slug = slug;
        Site = site;
    }
}
