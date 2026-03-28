namespace Application.Models.Responses;

public class TheProtocolItResponse
{
    public TheProtocolItPage Page { get; set; } = null!;
    public int OffersCount { get; set; }
    public List<TheProtocolItOffer> Offers { get; set; } = [];
    public TheProtocolItFilters Filters { get; set; } = null!;
    public TheProtocolItOrderBy OrderBy { get; set; } = null!;
}

public class TheProtocolItPage
{
    public int Number { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
}

public class TheProtocolItOffer
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string Title { get; set; } = null!;
    public string Employer { get; set; } = null!;
    public string EmployerId { get; set; } = null!;
    public string LogoUrl { get; set; } = null!;
    public string OfferUrlName { get; set; } = null!;
    public List<string> AboutProject { get; set; } = [];
    public List<TheProtocolItWorkplace> Workplace { get; set; } = [];
    public List<TheProtocolItPositionLevel> PositionLevels { get; set; } = [];
    public List<TheProtocolItTypeOfContract> TypesOfContracts { get; set; } = [];
    public List<string> Technologies { get; set; } = [];
    public bool New { get; set; }
    public DateTime PublicationDateUtc { get; set; }
    public bool LastCall { get; set; }
    public string Language { get; set; } = null!;
    public TheProtocolItSalary? Salary { get; set; }
    public List<string> WorkModes { get; set; } = [];
    public bool ImmediateEmployment { get; set; }
    public bool IsSupportingUkraine { get; set; }
    public TheProtocolItAddons Addons { get; set; } = null!;
    public bool IsFromExternalLocations { get; set; }
    public TheProtocolItBadges Badges { get; set; } = null!;
    public object? Alpha { get; set; }
}

public class TheProtocolItWorkplace
{
    public string Location { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Region { get; set; } = null!;
}

public class TheProtocolItPositionLevel
{
    public string Value { get; set; } = null!;
}

public class TheProtocolItTypeOfContract
{
    public int Id { get; set; }
    public TheProtocolItContractSalary? Salary { get; set; }
}

public class TheProtocolItContractSalary
{
    public decimal? From { get; set; }
    public decimal? To { get; set; }
    public string CurrencySymbol { get; set; } = null!;
    public int TimeUnitId { get; set; }
    public TheProtocolItTimeUnit TimeUnit { get; set; } = null!;
    public string KindName { get; set; } = null!;
}

public class TheProtocolItSalary
{
    public decimal? To { get; set; }
    public string Currency { get; set; } = null!;
    public TheProtocolItTimeUnit TimeUnit { get; set; } = null!;
}

public class TheProtocolItTimeUnit
{
    public string ShortForm { get; set; } = null!;
    public string LongForm { get; set; } = null!;
}

public class TheProtocolItAddons
{
    public List<object> SearchableLocations { get; set; } = [];
    public List<object> SearchableRegions { get; set; } = [];
    public bool IsWholePoland { get; set; }
}

public class TheProtocolItBadges
{
    public bool New { get; set; }
    public bool LastCall { get; set; }
    public bool ImmediateEmployment { get; set; }
    public bool IsSupportingUkraine { get; set; }
    public bool IsFromExternalLocations { get; set; }
    public bool IsQuickApply { get; set; }
}

public class TheProtocolItFilters
{
    public List<object> Cities { get; set; } = [];
    public List<object> RegionsOfWorld { get; set; } = [];
    public List<object> Technologies { get; set; } = [];
    public List<object> ExpectedTechnologies { get; set; } = [];
    public List<object> ExcludedTechnologies { get; set; } = [];
    public List<object> NiceToHaveTechnologies { get; set; } = [];
    public List<object> Keywords { get; set; } = [];
    public List<object> SpecializationsCodes { get; set; } = [];
}

public class TheProtocolItOrderBy
{
    public string Field { get; set; } = null!;
    public string Direction { get; set; } = null!;
}
