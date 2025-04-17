using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain;
public class GatherItDbContext : DbContext
{
    public GatherItDbContext(DbContextOptions<GatherItDbContext> options) : base(options)
    {
    }

    public DbSet<JobAd> JobAds { get; set; }
    public DbSet<CompanyName> CompanyNames { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Company> Companies { get; set; }
}
