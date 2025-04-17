using System.Collections.Frozen;
using Application.Dtos;
using Application.Interfaces.Repositories.Write;
using Domain;
using Domain.Entities;
using EFCore.BulkExtensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class WriteJobRepository : IWriteJobAdRepository
{
    private readonly GatherItDbContext _context;
    public WriteJobRepository(GatherItDbContext context)
    {
        _context = context;
    }

    public async Task AddDescription(FrozenDictionary<int, string> descriptions)
    {
        var entitiesToUpdate = await _context.JobAds
        .Where(jobAd => descriptions.Keys.Contains(jobAd.Id))
        .AsTracking() // Ensure entities are tracked
        .ToListAsync();

        // 2. Update the entities in memory
        foreach (var entity in entitiesToUpdate)
        {
            if (descriptions.TryGetValue(entity.Id, out var newDescription))
            {
                entity.Description = newDescription;
            }
        }

        // 3. Bulk mark as modified (more efficient than individual tracking)
        _context.UpdateRange(entitiesToUpdate);

        // 4. Single database operation
        await _context.SaveChangesAsync();
    }

    public async Task<List<JobAd>> InsertJobAds(IEnumerable<JobAdCreateDto> jobAdCreateDtos, bool doBulkInsert = false)
    {
        var jobAds = jobAdCreateDtos.Select(x => x.ToEntity());
        if (doBulkInsert)
        {
            var batches = jobAds.Chunk(500);

            foreach (var batch in batches)
                await _context.BulkInsertAsync(batch);

            return jobAds.ToList();
        }

        await _context.JobAds.AddRangeAsync(jobAds);
        await _context.SaveChangesAsync();

        return jobAds.ToList();
    }
}
