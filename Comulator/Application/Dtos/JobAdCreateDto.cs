using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos;

public class JobAdCreateDto
{
    [Required(ErrorMessage = "Job name is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Job name must be between 1 and 500 characters.")]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Remote type is required.")]
    public RemoteType RemoteType { get; set; }

    [Range(0, 100, ErrorMessage = "Remote percent must be between 0 and 100.")]
    public int? RemotePercent { get; set; }

    public IEnumerable<City> Cities { get; set; }

    public IEnumerable<Salary> Salaries { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Company name ID must be greater than zero.")]
    public int CompanyNameId { get; set; }

    public CompanyName CompanyName { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Slug must be between 1 and 500 characters.")]
    public string Slug { get; set; }

    [Required(ErrorMessage = "Site is required.")]
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
