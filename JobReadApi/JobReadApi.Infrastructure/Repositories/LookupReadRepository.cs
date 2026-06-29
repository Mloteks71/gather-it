using Microsoft.EntityFrameworkCore;
using JobReadApi.Application.Dtos;
using JobReadApi.Application.Interfaces;
using JobReadApi.Infrastructure.Data;

namespace JobReadApi.Infrastructure.Repositories;

public class LookupReadRepository(ReadApiDbContext dbContext) : ILookupReadRepository
{
    public async Task<IReadOnlyCollection<LookupItemDto>> GetSkillsAsync(string? search, int take, CancellationToken cancellationToken)
    {
        var query = dbContext.Skills.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{search}%"));
        }

        return await query
            .OrderBy(x => x.Name)
            .Take(take)
            .Select(x => new LookupItemDto(x.SkillId, x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LookupItemDto>> GetCompaniesAsync(string? search, int take, CancellationToken cancellationToken)
    {
        var query = dbContext.Companies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{search}%"));
        }

        return await query
            .OrderBy(x => x.Name)
            .Take(take)
            .Select(x => new LookupItemDto(x.CompanyId, x.Name))
            .ToListAsync(cancellationToken);
    }
}

