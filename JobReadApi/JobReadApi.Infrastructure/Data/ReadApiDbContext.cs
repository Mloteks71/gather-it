using Microsoft.EntityFrameworkCore;
using JobReadApi.Application.Entities;
using JobReadApi.Application.Enums;

namespace JobReadApi.Infrastructure.Data;

public class ReadApiDbContext(DbContextOptions<ReadApiDbContext> options) : DbContext(options)
{
    public DbSet<JobAd> JobAds => Set<JobAd>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillVariant> SkillVariants => Set<SkillVariant>();
    public DbSet<SkillSnapshot> SkillSnapshots => Set<SkillSnapshot>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyVariant> CompanyVariants => Set<CompanyVariant>();
    public DbSet<CompanySnapshot> CompanySnapshots => Set<CompanySnapshot>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<JobAdSkill> JobAdSkills => Set<JobAdSkill>();
    public DbSet<Description> Descriptions => Set<Description>();
    public DbSet<Salary> Salaries => Set<Salary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // HasPostgresEnum: no args = auto snake_case name, default public schema
        // MapEnum on NpgsqlDataSource in DependencyInjection.cs handles Npgsql serialization
        modelBuilder.HasPostgresEnum<JobSite>();
        modelBuilder.HasPostgresEnum<ContractType>();
        modelBuilder.HasPostgresEnum<WorkplaceType>();
        modelBuilder.HasPostgresEnum<ExperienceLevel>();
        modelBuilder.HasPostgresEnum<OfferStatus>();

        modelBuilder.Entity<JobAd>(entity =>
        {
            entity.ToTable("job_ad");
            entity.HasKey(x => x.JobAdId);
            entity.Property(x => x.JobAdId).HasColumnName("job_ad_id");
            entity.Property(x => x.ExternalId).HasColumnName("external_id").HasMaxLength(255);
            entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(500);
            entity.Property(x => x.OfferStatus).HasColumnName("offer_status");
            entity.Property(x => x.WorkplaceType).HasColumnName("workplace_type");
            entity.Property(x => x.ExperienceLevel).HasColumnName("experience_level");
            entity.Property(x => x.CompanyId).HasColumnName("company_id");
            entity.Property(x => x.JobSite).HasColumnName("job_site");
            entity.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(500);
            entity.Property(x => x.ExpiredAt).HasColumnName("expired_at");
            entity.Property(x => x.PublishedAt).HasColumnName("published_at");

            entity.HasOne(x => x.Company)
                .WithMany(x => x.JobAds)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("skill");
            entity.HasKey(x => x.SkillId);
            entity.Property(x => x.SkillId).HasColumnName("skill_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
        });

        modelBuilder.Entity<SkillVariant>(entity =>
        {
            entity.ToTable("skill_variant");
            entity.HasKey(x => x.SkillVariantId);
            entity.Property(x => x.SkillVariantId).HasColumnName("skill_variant_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(x => x.SkillId).HasColumnName("skill_id");

            entity.HasOne(x => x.Skill)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SkillSnapshot>(entity =>
        {
            entity.ToTable("skill_snapshot");
            entity.HasKey(x => x.SkillSnapshotId);
            entity.Property(x => x.SkillSnapshotId).HasColumnName("skill_snapshot_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(x => x.JobAdIds).HasColumnName("job_ad_ids");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("company");
            entity.HasKey(x => x.CompanyId);
            entity.Property(x => x.CompanyId).HasColumnName("company_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
        });

        modelBuilder.Entity<CompanyVariant>(entity =>
        {
            entity.ToTable("company_variant");
            entity.HasKey(x => x.CompanyVariantId);
            entity.Property(x => x.CompanyVariantId).HasColumnName("company_variant_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(x => x.CompanyId).HasColumnName("company_id");

            entity.HasOne(x => x.Company)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CompanySnapshot>(entity =>
        {
            entity.ToTable("company_snapshot");
            entity.HasKey(x => x.CompanySnapshotId);
            entity.Property(x => x.CompanySnapshotId).HasColumnName("company_snapshot_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(x => x.JobAdIds).HasColumnName("job_ad_ids");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");
            entity.HasKey(x => x.CityId);
            entity.Property(x => x.CityId).HasColumnName("city_id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100);
        });

        modelBuilder.Entity<JobAdSkill>(entity =>
        {
            entity.ToTable("job_ad_skill");
            entity.HasKey(x => new { x.JobAdId, x.SkillId });
            entity.Property(x => x.JobAdId).HasColumnName("job_ad_id");
            entity.Property(x => x.SkillId).HasColumnName("skill_id");

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
            entity.ToTable("description");
            entity.HasKey(x => x.DescriptionId);
            entity.Property(x => x.DescriptionId).HasColumnName("description_id");
            entity.Property(x => x.JobAdId).HasColumnName("job_ad_id");
            entity.Property(x => x.DescriptionText).HasColumnName("description_text");
            entity.Property(x => x.Requirements).HasColumnName("requirements");
            entity.Property(x => x.Benefits).HasColumnName("benefits");
            entity.Property(x => x.Workstyle).HasColumnName("workstyle");
            entity.Property(x => x.AboutProject).HasColumnName("about_project");

            entity.HasOne(x => x.JobAd)
                .WithMany(x => x.Descriptions)
                .HasForeignKey(x => x.JobAdId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.ToTable("salary");
            entity.HasKey(x => x.SalaryId);
            entity.Property(x => x.SalaryId).HasColumnName("salary_id");
            entity.Property(x => x.ContractType).HasColumnName("contract_type");
            entity.Property(x => x.SalaryMin).HasColumnName("salary_min");
            entity.Property(x => x.SalaryMax).HasColumnName("salary_max");
            entity.Property(x => x.JobAdId).HasColumnName("job_ad_id");

            entity.HasOne(x => x.JobAd)
                .WithMany(x => x.Salaries)
                .HasForeignKey(x => x.JobAdId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

