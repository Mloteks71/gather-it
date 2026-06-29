using JobReadApi.Application.Enums;

namespace JobReadApi.Application.Dtos;

public record SalaryDto(
    int SalaryId,
    ContractType ContractType,
    float? SalaryMin,
    float? SalaryMax);

