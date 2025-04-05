using Domain.Entities;
using Domain.Enums;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace Application.Dtos.TheProtocolIt;
public class TheProtocolItResponse
{
    [JsonPropertyName("page")]
    public required PageInfo Page { get; set; }

    [JsonPropertyName("offersCount")]
    public int OffersCount { get; set; }

    [JsonPropertyName("offers")]
    public required List<Offer> Offers { get; set; }

    [JsonPropertyName("filters")]
    public required Filters FiltersObject { get; set; }

    [JsonPropertyName("orderBy")]
    public required OrderBy OrderByObject { get; set; }


    #region mappings
    public IEnumerable<JobAdCreateDto> GenerateJobAdCreateDtos()
    {
        var companyNames = Offers
            .DistinctBy(x => x.Employer)
            .Select(x => new CompanyName(x.Employer))
            .ToHashSet();

        var cities = Offers
            .Where(x => x.Workplace is not null)
            .SelectMany(x => x.Workplace!)
            .DistinctBy(y => y.City, StringComparer.InvariantCultureIgnoreCase)
            .Select(y => new City(y.City))
            .ToFrozenDictionary(
                x => x,
                x => Offers
                    .Where(y => y.Workplace!
                        .FirstOrDefault(z => z.City == x.Name) is not null)
                );

        var citySlugLookup = cities
            .SelectMany(x => x.Value
                .Select(y => (y.OfferUrlName, x.Key)))
            .ToLookup(x => x.OfferUrlName, x => x.Key);

        //if (Offers.Contains(x => x.))
        //{

        //}

        return Offers.Select(x =>
        {
            var citiesKeys = citySlugLookup[x.OfferUrlName];

            var salariesKeys = (x.TypesOfContracts ?? [])
                .Select(z =>
                    new Salary(
                        MapToContractType(z.Id),
                        Convert.ToInt32(z.Salary?.From),
                        Convert.ToInt32(z.Salary?.To)
                        )
                );

            var companyName = companyNames.First(y => y.Name == x.Employer);

            return new JobAdCreateDto(
                name: x.Title,
                description: string.Empty,
                remoteType: MapToRemoteType(x.WorkModes),
                remotePercent: null,
                cities: citiesKeys,
                salaries: salariesKeys,
                companyNameId: companyName.Id,
                companyName: companyName,
                slug: x.OfferUrlName
            );
        });
    }

    private static readonly Dictionary<int, ContractType> ContractTypeMap = new()
    {
        { 0, ContractType.UoP },
        { 1, ContractType.UZ },//+UD
        { 2, ContractType.UZ },
        { 3, ContractType.B2B },
        { 4, ContractType.Replacement },
        { 5, ContractType.Undefined },
        { 6, ContractType.Replacement },
        { 7, ContractType.Internship }
    };

    private static ContractType MapToContractType(int input)
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

    private static RemoteType MapToRemoteType(List<string> remoteTypes)
    {
        RemoteType result = RemoteType.None;

        foreach (string remoteType in remoteTypes)
        {
            if (RemoteTypeMap.TryGetValue(remoteType, out RemoteType flag))
            {
                result |= flag; // Combine flags using bitwise OR
            }
        }

        return result;
    }

    #endregion

    #region classes
    public class PageInfo
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class Offer
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("groupId")]
        public required string GroupId { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("employer")]
        public required string Employer { get; set; }

        [JsonPropertyName("employerId")]
        public required string EmployerId { get; set; }

        [JsonPropertyName("logoUrl")]
        public required string LogoUrl { get; set; }

        [JsonPropertyName("offerUrlName")]
        public required string OfferUrlName { get; set; }

        [JsonPropertyName("aboutProject")]
        public required List<string> AboutProject { get; set; }

        [JsonPropertyName("workplace")]
        public required List<Workplace> Workplace { get; set; }

        [JsonPropertyName("positionLevels")]
        public required List<PositionLevel> PositionLevels { get; set; }

        [JsonPropertyName("typesOfContracts")]
        public required List<ContractTypeTheProtocolIt> TypesOfContracts { get; set; }

        [JsonPropertyName("technologies")]
        public required List<string> Technologies { get; set; }

        [JsonPropertyName("new")]
        public bool IsNew { get; set; }

        [JsonPropertyName("publicationDateUtc")]
        public DateTime PublicationDateUtc { get; set; }

        [JsonPropertyName("lastCall")]
        public bool LastCall { get; set; }

        [JsonPropertyName("language")]
        public required string Language { get; set; }

        [JsonPropertyName("salary")]
        public SalaryInfo? Salary { get; set; }

        [JsonPropertyName("workModes")]
        public required List<string> WorkModes { get; set; }

        [JsonPropertyName("immediateEmployment")]
        public bool ImmediateEmployment { get; set; }

        [JsonPropertyName("isSupportingUkraine")]
        public bool IsSupportingUkraine { get; set; }

        [JsonPropertyName("addons")]
        public Addons? Addons { get; set; }

        [JsonPropertyName("isFromExternalLocations")]
        public bool IsFromExternalLocations { get; set; }
    }

    public class Workplace
    {
        [JsonPropertyName("location")]
        public required string Location { get; set; }

        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("region")]
        public required string Region { get; set; }
    }

    public class PositionLevel
    {
        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }

    public class ContractTypeTheProtocolIt
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("salary")]
        public SalaryDetail? Salary { get; set; }
    }

    public class SalaryDetail
    {
        [JsonPropertyName("from")]
        public double? From { get; set; }

        [JsonPropertyName("to")]
        public double? To { get; set; }

        [JsonPropertyName("currencySymbol")]
        public required string CurrencySymbol { get; set; }

        [JsonPropertyName("timeUnitId")]
        public int TimeUnitId { get; set; }

        [JsonPropertyName("timeUnit")]
        public required TimeUnit TimeUnit { get; set; }

        [JsonPropertyName("kindName")]
        public string? KindName { get; set; }
    }

    public class TimeUnit
    {
        [JsonPropertyName("shortForm")]
        public required string ShortForm { get; set; }

        [JsonPropertyName("longForm")]
        public required string LongForm { get; set; }
    }

    public class SalaryInfo
    {
        [JsonPropertyName("to")]
        public double To { get; set; }

        [JsonPropertyName("currency")]
        public required string Currency { get; set; }

        [JsonPropertyName("timeUnit")]
        public required TimeUnit TimeUnit { get; set; }
    }

    public class Addons
    {
        //[JsonPropertyName("searchableLocations")]
        //public required List<string> SearchableLocations { get; set; }

        //[JsonPropertyName("searchableRegions")]
        //public required List<string> SearchableRegions { get; set; }

        [JsonPropertyName("isWholePoland")]
        public bool IsWholePoland { get; set; }
    }

    public class Filters
    {
        [JsonPropertyName("cities")]
        public required List<string> Cities { get; set; }

        [JsonPropertyName("regionsOfWorld")]
        public required List<string> RegionsOfWorld { get; set; }

        [JsonPropertyName("technologies")]
        public required List<string> Technologies { get; set; }

        [JsonPropertyName("keywords")]
        public required List<string> Keywords { get; set; }

        [JsonPropertyName("specializationsCodes")]
        public required List<string> SpecializationsCodes { get; set; }
    }

    public class OrderBy
    {
        [JsonPropertyName("field")]
        public required string Field { get; set; }

        [JsonPropertyName("direction")]
        public required string Direction { get; set; }
    }
    #endregion
}