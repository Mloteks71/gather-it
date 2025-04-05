using Application.Dtos;
using Domain.Entities;

namespace Infrastructure.Mappers;

public static class JobAdMapper
{
    public static JobAd ToEntity(this JobAdCreateDto jobAdCreateDto)
    {
        return new JobAd
        {
            Name = jobAdCreateDto.Name,
            Description = jobAdCreateDto.Description,
            RemoteType = jobAdCreateDto.RemoteType,
            RemotePercent = jobAdCreateDto.RemotePercent,
            Cities = jobAdCreateDto.Cities.ToList(),
            Salaries = jobAdCreateDto.Salaries.ToList(),
            CompanyNameId = jobAdCreateDto.CompanyNameId,
            CompanyName = jobAdCreateDto.CompanyName,
            Slug = jobAdCreateDto.Slug,
        };
    }
}