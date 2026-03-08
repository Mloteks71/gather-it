using System.Text.Json.Serialization;

namespace Application.Models.Responses;

public class JustJoinItResponse
{
    [JsonPropertyName("data")]
    public List<JobAd> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public MetaData Meta { get; set; } = new();
}

public class JobAd
{
    [JsonPropertyName("guid")]
    public string Guid { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("requiredSkills")]
    public List<string> RequiredSkills { get; set; } = new();

    [JsonPropertyName("niceToHaveSkills")]
    public List<string>? NiceToHaveSkills { get; set; }

    [JsonPropertyName("workplaceType")]
    public string WorkplaceType { get; set; } = string.Empty;

    [JsonPropertyName("workingTime")]
    public string WorkingTime { get; set; } = string.Empty;

    [JsonPropertyName("experienceLevel")]
    public string ExperienceLevel { get; set; } = string.Empty;

    [JsonPropertyName("employmentTypes")]
    public List<EmploymentType> EmploymentTypes { get; set; } = new();

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("multilocation")]
    public List<Location> Multilocation { get; set; } = new();

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    [JsonPropertyName("remoteInterview")]
    public bool RemoteInterview { get; set; }

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("companyLogoThumbUrl")]
    public string CompanyLogoThumbUrl { get; set; } = string.Empty;

    [JsonPropertyName("publishedAt")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("lastPublishedAt")]
    public DateTime LastPublishedAt { get; set; }

    [JsonPropertyName("expiredAt")]
    public DateTime ExpiredAt { get; set; }

    [JsonPropertyName("openToHireUkrainians")]
    public bool OpenToHireUkrainians { get; set; }

    [JsonPropertyName("languages")]
    public List<Language> Languages { get; set; } = new();

    [JsonPropertyName("applyMethod")]
    public string ApplyMethod { get; set; } = string.Empty;

    [JsonPropertyName("isSuperOffer")]
    public bool IsSuperOffer { get; set; }

    [JsonPropertyName("promotedPosition")]
    public int? PromotedPosition { get; set; }

    [JsonPropertyName("promotedKeyFilters")]
    public List<string> PromotedKeyFilters { get; set; } = new();
}

public class EmploymentType
{
    [JsonPropertyName("from")]
    public decimal? From { get; set; }

    [JsonPropertyName("to")]
    public decimal? To { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("gross")]
    public bool Gross { get; set; }

    [JsonPropertyName("fromChf")]
    public decimal? FromChf { get; set; }

    [JsonPropertyName("fromEur")]
    public decimal? FromEur { get; set; }

    [JsonPropertyName("fromGbp")]
    public decimal? FromGbp { get; set; }

    [JsonPropertyName("fromPln")]
    public decimal? FromPln { get; set; }

    [JsonPropertyName("fromUsd")]
    public decimal? FromUsd { get; set; }

    [JsonPropertyName("toChf")]
    public decimal? ToChf { get; set; }

    [JsonPropertyName("toEur")]
    public decimal? ToEur { get; set; }

    [JsonPropertyName("toGbp")]
    public decimal? ToGbp { get; set; }

    [JsonPropertyName("toPln")]
    public decimal? ToPln { get; set; }

    [JsonPropertyName("toUsd")]
    public decimal? ToUsd { get; set; }
}

public class Location
{
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class Language
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;
}

public class MetaData
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("prevPage")]
    public int? PrevPage { get; set; }

    [JsonPropertyName("nextPage")]
    public int? NextPage { get; set; }
}
