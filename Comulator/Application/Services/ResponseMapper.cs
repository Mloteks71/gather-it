using Application.Interfaces;
using Application.Models.Dtos;
using Application.Models.Responses;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ResponseMapper : IResponseMapper
{
    private readonly ILogger<ResponseMapper> _logger;

    public ResponseMapper(ILogger<ResponseMapper> logger)
    {
        _logger = logger;
    }

    public IEnumerable<CommonJobAdDto> MapJustJoinItResponse(JustJoinItResponse response)
    {
        _logger.LogInformation("Mapping {Count} job ads from JustJoinIt", response.Data.Count);

        var mappedAds = response.Data.Select(jobAd => new CommonJobAdDto
        {
            Id = jobAd.Guid,
            Slug = jobAd.Slug,
            Title = jobAd.Title,
            CompanyName = jobAd.CompanyName,
            SourceSite = Site.JustJoinIt,
            Skills = jobAd.RequiredSkills,
            WorkplaceTypes = string.IsNullOrWhiteSpace(jobAd.WorkplaceType)
                ? null
                : [jobAd.WorkplaceType],
            ExperienceLevels = string.IsNullOrWhiteSpace(jobAd.ExperienceLevel)
                ? null
                : [jobAd.ExperienceLevel],
            Locations = (jobAd.Multilocation?
                .Select(l => l.City)
                ?? Enumerable.Empty<string?>())
                .Append(jobAd.City)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c!)
                .Distinct()
                .ToList() is { Count: > 0 } locations
                ? locations
                : null,
            Salaries = jobAd.EmploymentTypes?.Select(sr => new SalaryRangeDto
            {
                From = sr.From,
                To = sr.To,
                Currency = sr.Currency,
                ContractType = sr.Type
            }).ToList(),
            PublishedAt = jobAd.PublishedAt,
            LogoUrl = jobAd.CompanyLogoThumbUrl
        });

        _logger.LogInformation(
            "Successfully mapped {Count} JustJoinIt job ads",
            mappedAds.Count());

        return mappedAds;
    }

    public IEnumerable<CommonJobAdDto> MapTheProtocolItResponse(TheProtocolItResponse response)
    {
        _logger.LogInformation("Mapping {Count} job ads from TheProtocol", response.Offers.Count);

        var mappedAds = response.Offers.Select(offer => new CommonJobAdDto
        {
            Id = offer.Id.ToString(),
            Slug = offer.OfferUrlName,
            Title = offer.Title,
            CompanyName = offer.Employer,
            SourceSite = Site.TheProtocolIt,
            Skills = offer.Technologies,
            WorkplaceTypes = offer.WorkModes,
            ExperienceLevels = offer.PositionLevels?.Select(pl => pl.Value).ToList(),
            Locations = offer.Workplace
                ?.Select(w => w.City)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList(),
            Salaries = offer.TypesOfContracts
                ?.Where(c => c.Salary != null)
                .Select(c => new SalaryRangeDto
                {
                    From = c.Salary!.From,
                    To = c.Salary.To,
                    Currency = c.Salary.CurrencySymbol ?? string.Empty,
                    ContractType = c.Salary.KindName
                })
                .DefaultIfEmpty(offer.Salary == null
                    ? null
                    : new SalaryRangeDto
                    {
                        From = null,
                        To = offer.Salary.To,
                        Currency = offer.Salary.Currency ?? string.Empty,
                        ContractType = null
                    })
                .Where(s => s != null)
                .Select(s => s!)
                .ToList() is { Count: > 0 } salaries
                ? salaries
                : null,
            PublishedAt = offer.PublicationDateUtc,
            LogoUrl = offer.LogoUrl
        });

        _logger.LogInformation(
            "Successfully mapped {Count} TheProtocol job ads",
            mappedAds.Count());

        return mappedAds;
    }
}
