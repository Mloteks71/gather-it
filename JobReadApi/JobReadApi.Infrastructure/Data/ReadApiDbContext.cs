using JobReadApi.Application.Entities;
using JobReadApi.Application.Enums;
using Microsoft.EntityFrameworkCore;

namespace JobReadApi.Infrastructure.Data;

public class ReadApiDbContext(DbContextOptions<ReadApiDbContext> options) : DbContext(options)
{
    public DbSet<JobAd> JobAd => Set<JobAd>();
    public DbSet<Skill> Skill => Set<Skill>();
    public DbSet<SkillVariant> SkillVariant => Set<SkillVariant>();
    public DbSet<SkillSnapshot> SkillSnapshot => Set<SkillSnapshot>();
    public DbSet<Company> Company => Set<Company>();
    public DbSet<CompanyVariant> CompanyVariant => Set<CompanyVariant>();
    public DbSet<CompanySnapshot> CompanySnapshot => Set<CompanySnapshot>();
    public DbSet<City> City => Set<City>();
    public DbSet<JobAdSkill> JobAdSkill => Set<JobAdSkill>();
    public DbSet<Description> Description => Set<Description>();
    public DbSet<Salary> Salary => Set<Salary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<JobSite>();
        modelBuilder.HasPostgresEnum<ContractType>();
        modelBuilder.HasPostgresEnum<WorkplaceType>();
        modelBuilder.HasPostgresEnum<ExperienceLevel>();
        modelBuilder.HasPostgresEnum<OfferStatus>();

        modelBuilder.Entity<JobAd>(entity =>
        {
            entity.HasKey(x => x.JobAdId);
            entity.Property(x => x.ExternalId).HasMaxLength(255);
            entity.Property(x => x.Title).HasMaxLength(500);
            entity.Property(x => x.Slug).HasMaxLength(500);

            entity.HasOne(x => x.Company)
                .WithMany(x => x.JobAds)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(x => x.SkillId);
            entity.Property(x => x.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<SkillVariant>(entity =>
        {
            entity.HasKey(x => x.SkillVariantId);
            entity.Property(x => x.Name).HasMaxLength(255);

            entity.HasOne(x => x.Skill)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SkillSnapshot>(entity =>
        {
            entity.HasKey(x => x.SkillSnapshotId);
            entity.Property(x => x.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(x => x.CompanyId);
            entity.Property(x => x.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<CompanyVariant>(entity =>
        {
            entity.HasKey(x => x.CompanyVariantId);
            entity.Property(x => x.Name).HasMaxLength(255);

            entity.HasOne(x => x.Company)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CompanySnapshot>(entity =>
        {
            entity.HasKey(x => x.CompanySnapshotId);
            entity.Property(x => x.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(x => x.CityId);
            entity.Property(x => x.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<JobAdSkill>(entity =>
        {
            entity.HasKey(x => new { x.JobAdId, x.SkillId });

            entity.HasOne(x => x.JobAd)
                .WithMany(x => x.JobAdSkills)
                .HasForeignKey(x => x.JobAdId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Skill)
                .WithMany(x => x.JobAdSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.SkillId).HasDatabaseName("idx_job_ad_skill_skill_id");
        });

        modelBuilder.Entity<Description>(entity =>
        {
            entity.HasKey(x => x.DescriptionId);

            entity.HasOne(x => x.JobAd)
                .WithMany(x => x.Descriptions)
                .HasForeignKey(x => x.JobAdId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.HasKey(x => x.SalaryId);

            entity.HasOne(x => x.JobAd)
                .WithMany(x => x.Salaries)
                .HasForeignKey(x => x.JobAdId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

