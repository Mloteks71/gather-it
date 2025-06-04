using System.Diagnostics;
using Application.Dtos;
using Application.Interfaces.Repositories.Write;
using Domain;
using Domain.Entities;
using EFCore.BulkExtensions;
using Infrastructure.Mappers;
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

    public async Task AddDescription(IEnumerable<DescriptionCreateDto> descriptions)
    {
        await _context.BulkInsertAsync(descriptions.Select(x => new Description(x.Id, x.Description, x.Requirements, x.Benefits, x.Workstyle, x.AboutProject)), new BulkConfig { SetOutputIdentity = true });
        _logger.LogInformation("Added {Count} descriptions to the database.", descriptions.Count());
        await _context.SaveChangesAsync();
    }

    public async Task<List<JobAd>> InsertJobAds(IEnumerable<JobAdCreateDto> jobAdCreateDtos, bool doBulkInsert = false)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var jobAds = jobAdCreateDtos.Select(x => x.ToEntity()).ToList(); // ToList is required to get Ids from db on insert

        if (doBulkInsert)
        {
            var batches = jobAds.Chunk(500);

            foreach (var batch in batches)
            {
                await _context.BulkInsertAsync(batch, new BulkConfig { SetOutputIdentity = true });
            }

            await _context.SaveChangesAsync();
        }
        else
        {
            await _context.JobAds.AddRangeAsync(jobAds);
            await _context.SaveChangesAsync();
        }

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation($"Time after time:{elapsedMilliseconds} bulk?: {doBulkInsert}");
        return jobAds.ToList();
    }
}
