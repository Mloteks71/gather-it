using System.Collections.Frozen;
using Application.Dtos;
using Application.Interfaces.Repositories.Write;
using Domain;
using Domain.Entities;
using EFCore.BulkExtensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Write;
public class WriteJobRepository : IWriteJobAdRepository
{
    private readonly GatherItDbContext _context;
    private readonly ILogger _logger;
    public WriteJobRepository(GatherItDbContext context, ILogger<IWriteJobAdRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddDescription(FrozenDictionary<int, string> descriptions)
    {
        var entitiesToUpdate = await _context.JobAds
        .Where(jobAd => descriptions.Keys.Contains(jobAd.Id))
        .AsTracking()
        .ToListAsync();

        foreach (var entity in entitiesToUpdate)
        {
            if (descriptions.TryGetValue(entity.Id, out var newDescription))
            {
                entity.Description = newDescription;
            }
        }

        _context.UpdateRange(entitiesToUpdate);

        await _context.SaveChangesAsync();
    }

    public async Task<List<JobAd>> InsertJobAds(IEnumerable<JobAdCreateDto> jobAdCreateDtos, bool doBulkInsert = false)
    {
        var jobAds = jobAdCreateDtos.Select(x => x.ToEntity()).ToList();
        jobAds = jobAds.Where(x => x.Site == Domain.Enums.Site.JustJoinIt).Chunk(50).First().ToList();
        if (doBulkInsert)
        {
            var batches = jobAds.Chunk(500);

            foreach (var batch in batches)
            {
                await _context.BulkInsertAsync(batch, new BulkConfig { SetOutputIdentity = true });
                _logger.LogInformation($"Batch Id = {batch[1].Id}");
            }

            await _context.SaveChangesAsync();
            return jobAds.ToList();
        }

        await _context.JobAds.AddRangeAsync(jobAds);
        await _context.SaveChangesAsync();
        var jobAdsList = jobAds.ToList();
        _logger.LogInformation($"No batch Id = {jobAdsList.FirstOrDefault()?.Id}");
        return jobAds.ToList();
    }
}
