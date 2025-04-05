using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.SolidJobs;

public class SolidJobsResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("jobOfferKey")]
    public string JobOfferKey { get; set; }

    [JsonPropertyName("division")]
    public string Division { get; set; }

    [JsonPropertyName("mainCategory")]
    public string MainCategory { get; set; }

    [JsonPropertyName("subCategory")]
    public string SubCategory { get; set; }

    [JsonPropertyName("jobTitle")]
    public string JobTitle { get; set; }

    [JsonPropertyName("experienceLevel")]
    public string ExperienceLevel { get; set; }

    [JsonPropertyName("minimalExperienceInField")]
    public int MinimalExperienceInField { get; set; }

    [JsonPropertyName("salaryRange")]
    public SalaryRange Salary { get; set; }

    [JsonPropertyName("normalizedSalaryRange")]
    public SalaryRange NormalizedSalaryRange { get; set; }

    [JsonPropertyName("isSalaryNormalized")]
    public bool IsSalaryNormalized { get; set; }

    [JsonPropertyName("workload")]
    public string Workload { get; set; }

    [JsonPropertyName("remotePossible")]
    public string RemotePossible { get; set; }

    [JsonPropertyName("requiredSkills")]
    public List<Skill> RequiredSkills { get; set; }

    [JsonPropertyName("requiredLanguages")]
    public List<Skill> RequiredLanguages { get; set; }

    [JsonPropertyName("validFrom")]
    public DateTime ValidFrom { get; set; }

    [JsonPropertyName("validTo")]
    public DateTime ValidTo { get; set; }

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; }

    [JsonPropertyName("companyAddress")]
    public string CompanyAddress { get; set; }

    [JsonPropertyName("companyCity")]
    public string CompanyCity { get; set; }

    [JsonPropertyName("companyLogoUrl")]
    public string CompanyLogoUrl { get; set; }

    [JsonPropertyName("jobOfferUrl")]
    public string JobOfferUrl { get; set; }

    [JsonPropertyName("jobOfferShortUrl")]
    public string JobOfferShortUrl { get; set; }

    [JsonPropertyName("officeLatitude")]
    public double OfficeLatitude { get; set; }

    [JsonPropertyName("officeLongitude")]
    public double OfficeLongitude { get; set; }

    [JsonPropertyName("isPremium")]
    public bool IsPremium { get; set; }

    public class SalaryRange
    {
        [JsonPropertyName("lowerBound")]
        public double LowerBound { get; set; }

        [JsonPropertyName("upperBound")]
        public double UpperBound { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("employmentType")]
        public string EmploymentType { get; set; }

        [JsonPropertyName("salaryPeriod")]
        public string SalaryPeriod { get; set; }
    }

    public class Skill
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("skillType")]
        public string SkillType { get; set; }

        [JsonPropertyName("skillLevel")]
        public string SkillLevel { get; set; }
    }

    public static IEnumerable<JobAdCreateDto> GenerateJobAdCreateDtos(IEnumerable<SolidJobsResponse> solidJobsResponses)
    {
        return solidJobsResponses
            .Select(x => new JobAdCreateDto(
                name: x.JobTitle,
                description: string.Empty,
                remoteType: MapToRemoteType(x.RemotePossible),
                remotePercent: null,
                cities: new List<City> { new(x.CompanyCity) },
                salaries: new List<Salary> { new (MapToContractType
                (
                    x.Salary.EmploymentType),
                    Convert.ToInt32(x.Salary.LowerBound),
                    Convert.ToInt32(x.Salary.UpperBound))
                },
                companyNameId: 0,
                companyName: new CompanyName(x.CompanyName),
                slug: $"{x.Id}/{x.JobOfferKey}"
            ));
    }

    private static RemoteType MapToRemoteType(string type)
    {
        var result = RemoteType.None;

        if (RemoteTypeMap.TryGetValue(type, out var flag))
        {
            result |= flag;
        }

        return result;
    }

    private static readonly Dictionary<string, RemoteType> RemoteTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Możliwa w całości", RemoteType.Remote },
        { "W całości", RemoteType.Remote },
        { "Brak", RemoteType.Stationary },
        { "Możliwa częściowo", RemoteType.Hybrid }
    };

    private static readonly Dictionary<string, ContractType> ContractTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Umowa o pracę", ContractType.UoP },
        { "B2B", ContractType.B2B },
        { "Umowa zlecenie", ContractType.UZ },
        { "Umowa o dzieło", ContractType.Contract }
    };

    private static ContractType MapToContractType(string input)
    {
        if (ContractTypeMap.TryGetValue(input, out var result))
        {
            return result;
        }
        throw new ArgumentException($"No enum value mapped to string: {input}");
    }
}

