using Domain.Entities;
using Domain.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Domain;
public class GatherItDbContext : DbContext
{
    public GatherItDbContext(DbContextOptions<GatherItDbContext> options) : base(options)
    {
    }

    public DbSet<JobAd> JobAds { get; set; }
    public DbSet<Description> Descriptions { get; set; }
    public DbSet<CompanyName> CompanyNames { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new JobAdConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyNameConfiguration());
        modelBuilder.ApplyConfiguration(new SalaryConfiguration());
        modelBuilder.ApplyConfiguration(new DescriptionConfiguration());
    }
}
