using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Mappers;

public static class JobAdMapper {
    public static JobAd ToEntity(this JobAdCreateDto jobAdCreateDto) {
        return new JobAd {
            Name = jobAdCreateDto.Name,
            Description = jobAdCreateDto.Description,
            RemoteType = jobAdCreateDto.RemoteType,
            RemotePercent = jobAdCreateDto.RemotePercent,
            Cities = jobAdCreateDto.Cities,
            Salaries = jobAdCreateDto.Salaries,
            CompanyNameId = jobAdCreateDto.CompanyNameId,
            CompanyName = jobAdCreateDto.CompanyName,
            Slug = jobAdCreateDto.Slug,
        };
    }
}