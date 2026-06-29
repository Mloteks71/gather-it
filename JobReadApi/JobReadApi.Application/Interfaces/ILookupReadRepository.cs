using JobReadApi.Application.Dtos;

namespace JobReadApi.Application.Interfaces;

public interface ILookupReadRepository
{
    Task<IReadOnlyCollection<LookupItemDto>> GetSkillsAsync(string? search, int take, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LookupItemDto>> GetCompaniesAsync(string? search, int take, CancellationToken cancellationToken);
}

