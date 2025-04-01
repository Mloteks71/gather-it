using System.Collections.Immutable;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.Dtos.JustJoinIt;
public class JustJoinItResponse
{
    [JsonPropertyName("data")]
    public required List<JustJoinItJob> Jobs { get; set; }
    [JsonPropertyName("meta")]
    public required JustJoinItMetadata MetaData { get; set; }

    public IEnumerable<JobAdCreateDto> GenerateJobAdCreateDtos() {
        var companyNames = Jobs
            .DistinctBy(x => x.CompanyName)
            .Select(x => new CompanyName(x.CompanyName))
            .ToImmutableHashSet();

        var cities = Jobs
            .Where(x => x.Multilocation is not null)
            .SelectMany(x => x.Multilocation!)
            .DistinctBy(y => y.City, StringComparer.InvariantCultureIgnoreCase)
            .Select(y => new City(y.City))
            .ToImmutableDictionary(
                x => x,
                x => Jobs
                    .Where(y => y.Multilocation!
                        .FirstOrDefault(z => z.City == x.Name) is not null)
                );

        var citySlugLookup = cities
            .SelectMany(x => x.Value
                .Select(y => (y.Slug, x.Key)))
            .ToLookup(x => x.Slug, x => x.Key);

        return Jobs.Select(x => {
            var citiesKeys = citySlugLookup[x.Slug];
            // var citiesKeys = cities
            //     .Where(y => y.Value
            //         .FirstOrDefault(z => z.Slug == x.Slug) is not null)
            //     .Select(y => y.Key);

            var salariesKeys = (x.EmploymentTypes ?? [])
                .Select(z =>
                    new Salary(
                        MapToContractType(z.Type),
                        Convert.ToInt32(z.FromPln),
                        Convert.ToInt32(z.ToPln)
                        )
                );

            var companyName = companyNames.First(y => y.Name == x.CompanyName);

            return new JobAdCreateDto(
                name: x.Title,
                description: string.Empty,
                remoteType: MapToRemoteType(x.WorkplaceType),
                remotePercent: null,
                cities: citiesKeys,
                salaries: salariesKeys,
                companyNameId: companyName.Id,
                companyName: companyName,
                slug: x.Slug
            );
        });
    }

    #region mappings

    private static readonly Dictionary<string, ContractType> ContractTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "permanent", ContractType.UoP },
        { "B2B", ContractType.B2B },
        { "mandate_contract", ContractType.UZ },
        { "any", ContractType.Any },
        { "internship", ContractType.Internship },
        { "contract", ContractType.Contract }
    };

    private static ContractType MapToContractType(string input)
    {
        if (ContractTypeMap.TryGetValue(input, out var result))
        {
            return result;
        }
        throw new ArgumentException($"No enum value mapped to string: {input}");
    }

    private static readonly Dictionary<string, RemoteType> RemoteTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Remote", RemoteType.Remote },
        { "Office", RemoteType.Stationary },
        { "Hybrid", RemoteType.Hybrid }
    };

    private static RemoteType MapToRemoteType(string type)
    {
        RemoteType result = RemoteType.None;

        if (RemoteTypeMap.TryGetValue(type, out RemoteType flag))
        {
            result |= flag; // Combine flags using bitwise OR
        }

        return result;
    }

    #endregion

    #region classes
    public class JustJoinItJob
    {
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("requiredSkills")]
        public List<string>? RequiredSkills { get; set; }

        [JsonPropertyName("niceToHaveSkills")]
        public List<string>? NiceToHaveSkills { get; set; }

        [JsonPropertyName("workplaceType")]
        public required string WorkplaceType { get; set; }

        [JsonPropertyName("workingTime")]
        public required string WorkingTime { get; set; }

        [JsonPropertyName("experienceLevel")]
        public required string ExperienceLevel { get; set; }

        [JsonPropertyName("employmentTypes")]
        public List<JustJoinItEmploymentType>? EmploymentTypes { get; set; }

        [JsonPropertyName("categoryId")]
        public required JustJoinItCategory CategoryId { get; set; }

        [JsonPropertyName("multilocation")]
        public List<JustJoinItMultilocation>? Multilocation { get; set; }

        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("street")]
        public required string Street { get; set; }

        [JsonPropertyName("latitude")]
        public required double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public required double? Longitude { get; set; }

        [JsonPropertyName("remoteInterview")]
        public required bool RemoteInterview { get; set; }

        [JsonPropertyName("companyName")]
        public required string CompanyName { get; set; }

        [JsonPropertyName("companyLogoThumbUrl")]
        public required string CompanyLogoThumbUrl { get; set; }

        [JsonPropertyName("publishedAt")]
        public required DateTime PublishedAt { get; set; }

        [JsonPropertyName("openToHireUkrainians")]
        public required bool OpenToHireUkrainians { get; set; }

        [JsonPropertyName("languages")]
        public List<JustJoinItLanguage>? Languages { get; set; }
    }

    public class JustJoinItMetadata
    {
        [JsonPropertyName("NextPage")]
        public required int? NextPage { get; set; }
        [JsonPropertyName("Page")]
        public required int Page { get; set; }
        [JsonPropertyName("PrevPage")]
        public required int? PrevPage { get; set; }
        [JsonPropertyName("TotalItems")]
        public required int? TotalItems { get; set; }
        [JsonPropertyName("TotalPages")]
        public required int TotalPages { get; set; }
    }

    public class JustJoinItEmploymentType
    {
        [JsonPropertyName("from")]
        public double? From { get; set; }

        [JsonPropertyName("to")]
        public double? To { get; set; }

        [JsonPropertyName("currency")]
        public required string Currency { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("unit")]
        public required string Unit { get; set; }

        [JsonPropertyName("gross")]
        public bool Gross { get; set; }

        [JsonPropertyName("fromChf")]
        public double? FromChf { get; set; }

        [JsonPropertyName("fromEur")]
        public double? FromEur { get; set; }

        [JsonPropertyName("fromGbp")]
        public double? FromGbp { get; set; }

        [JsonPropertyName("fromPln")]
        public double? FromPln { get; set; }

        [JsonPropertyName("fromUsd")]
        public double? FromUsd { get; set; }

        [JsonPropertyName("toChf")]
        public double? ToChf { get; set; }

        [JsonPropertyName("toEur")]
        public double? ToEur { get; set; }

        [JsonPropertyName("toGbp")]
        public double? ToGbp { get; set; }

        [JsonPropertyName("toPln")]
        public double? ToPln { get; set; }

        [JsonPropertyName("toUsd")]
        public double? ToUsd { get; set; }
    }

    public class JustJoinItMultilocation
    {
        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("street")]
        public required string Street { get; set; }

        [JsonPropertyName("latitude")]
        public required double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public required double? Longitude { get; set; }
    }

    public class JustJoinItLanguage
    {
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [JsonPropertyName("level")]
        public required string Level { get; set; }
    }
    #endregion
}