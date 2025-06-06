﻿using Application.Interfaces.Repositories.Read;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories.Read;
public class ReadJobRepository : IReadJobAdRepository
{
    private readonly GatherItDbContext _context;
    public ReadJobRepository(GatherItDbContext context)
    {
        _context = context;
    }

    public IEnumerable<JobAd> GetJobAds(IEnumerable<string> companyNames)
    {
        var companyNamesSet = new HashSet<string>(companyNames, StringComparer.OrdinalIgnoreCase);

        return _context.JobAds.Where(x =>
            x.CompanyName != null
             && x.CompanyName.Company != null
             && x.CompanyName.Company.Names != null
             && x.CompanyName.Company.Names.Select(n => n.Name).Any(name => companyNamesSet.Contains(name)) ||
            x.CompanyName != null
             && x.CompanyName.Company == null
             && companyNamesSet.Contains(x.CompanyName.Name));
    }

    public IEnumerable<string> GetJobAdsSlug()
    {
        return _context.JobAds
            .Select(x => x.Slug)
            .ToHashSet();
    }
}
