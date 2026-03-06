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

    public List<CommonJobAdDto> MapJustJoinItResponse(JustJoinItResponse response)
    {
        _logger.LogInformation("Mapping {Count} job ads from JustJoinIt", response.Data.Count);

        var mappedAds = response.Data.Select(jobAd => new CommonJobAdDto
        {
            Id = jobAd.Guid,
            Slug = jobAd.Slug,
            Title = jobAd.Title,
            CompanyName = jobAd.CompanyName,
            SourceSite = Site.JustJoinIt,
            RequiredSkills = jobAd.RequiredSkills,
            NiceToHaveSkills = jobAd.NiceToHaveSkills,
            WorkplaceTypes = string.IsNullOrEmpty(jobAd.WorkplaceType)
                ? null
                : [jobAd.WorkplaceType],
            ExperienceLevels = string.IsNullOrEmpty(jobAd.ExperienceLevel)
                ? null
                : [jobAd.ExperienceLevel],
            Locations = GetJustJoinItLocations(jobAd),
            Salaries = jobAd.EmploymentTypes?.Select(sr => new SalaryRangeDto
            {
                From = sr.From,
                To = sr.To,
                Currency = sr.Currency,
                ContractType = sr.Type
            }).ToList(),
            PublishedAt = jobAd.PublishedAt,
            Technologies = null,
            LogoUrl = jobAd.CompanyLogoThumbUrl
        }).ToList();

        _logger.LogInformation(
            "Successfully mapped {Count} JustJoinIt job ads",
            mappedAds.Count);

        return mappedAds;
    }

    public List<CommonJobAdDto> MapTheProtocolItResponse(TheProtocolItResponse response)
    {
        _logger.LogInformation("Mapping {Count} job ads from TheProtocol", response.Offers.Count);

        var mappedAds = response.Offers.Select(offer => new CommonJobAdDto
        {
            Id = offer.Id.ToString(),
            Slug = offer.OfferUrlName,
            Title = offer.Title,
            CompanyName = offer.Employer,
            SourceSite = Site.TheProtocolIt,
            RequiredSkills = null,
            NiceToHaveSkills = null,
            WorkplaceTypes = offer.WorkModes,
            ExperienceLevels = offer.PositionLevels?.Select(pl => pl.Value).ToList(),
            Locations = offer.Workplace
                ?.Select(w => w.City)
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList(),
            Salaries = MapTheProtocolSalaries(offer),
            PublishedAt = offer.PublicationDateUtc,
            Technologies = offer.Technologies,
            LogoUrl = offer.LogoUrl
        }).ToList();

        _logger.LogInformation(
            "Successfully mapped {Count} TheProtocol job ads",
            mappedAds.Count);

        return mappedAds;
    }

    private static List<string>? GetJustJoinItLocations(JobAd jobAd)
    {
        var locations = new List<string>();

        if (jobAd.Multilocation != null)
        {
            var multiLocationCities = jobAd.Multilocation
                .Select(l => l.City)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            locations.AddRange(multiLocationCities);
        }

        if (!string.IsNullOrEmpty(jobAd.City) && !locations.Contains(jobAd.City))
        {
            locations.Add(jobAd.City);
        }

        return locations.Count > 0 ? locations : null;
    }

    private static List<SalaryRangeDto>? MapTheProtocolSalaries(TheProtocolItOffer offer)
    {
        var salaries = new List<SalaryRangeDto>();

        if (offer.TypesOfContracts != null)
        {
            foreach (var contract in offer.TypesOfContracts.Where(c => c.Salary != null))
            {
                salaries.Add(new SalaryRangeDto
                {
                    From = contract.Salary!.From,
                    To = contract.Salary.To,
                    Currency = contract.Salary.CurrencySymbol ?? string.Empty,
                    ContractType = contract.Salary.KindName
                });
            }
        }

        if (offer.Salary != null && salaries.Count == 0)
        {
            salaries.Add(new SalaryRangeDto
            {
                From = null,
                To = offer.Salary.To,
                Currency = offer.Salary.Currency ?? string.Empty,
                ContractType = null
            });
        }

        return salaries.Count > 0 ? salaries : null;
    }
}
