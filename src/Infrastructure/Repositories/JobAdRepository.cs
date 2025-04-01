using Application.Dtos;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Entities;
using EFCore.BulkExtensions;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;
public class JobAdRepository : IJobAdRepository {
    private readonly GatherItDbContext _context;
    public JobAdRepository(GatherItDbContext context) {
        _context = context;
    }

    public IEnumerable<JobAd> GetJobAds(IEnumerable<string> companyNames)
    {
        var companyNamesSet = new HashSet<string>(companyNames, StringComparer.OrdinalIgnoreCase);
    
        return _context.JobAds.Where(x =>
            (x.CompanyName != null
             && x.CompanyName.Company != null
             && x.CompanyName.Company.Names != null
             && x.CompanyName.Company.Names.Select(n => n.Name).Any(name => companyNamesSet.Contains(name))) ||
            (x.CompanyName != null
             && x.CompanyName.Company == null
             && companyNamesSet.Contains(x.CompanyName.Name)));
    }

    public IEnumerable<string> GetJobAdsSlug(IEnumerable<string> slugs) {
        return  _context.JobAds
            .Select(x => x.Slug)
            .ToHashSet();
    }

    public async Task InsertJobAds(IEnumerable<JobAdCreateDto> jobAds, bool doBulkInsert = false) {
        if(doBulkInsert)
        {
            var batches = jobAds.Select(x => x.ToEntity()).Chunk(500);
            
            foreach (var batch in batches)
                await _context.BulkInsertAsync(batch);
            
            return;
        }
        
        await _context.JobAds.AddRangeAsync(jobAds.Select(x => x.ToEntity()));
        await _context.SaveChangesAsync();
    }
}
