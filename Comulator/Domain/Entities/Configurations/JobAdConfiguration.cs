using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class JobAdConfiguration : IEntityTypeConfiguration<JobAd>
{
    public void Configure(EntityTypeBuilder<JobAd> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        // String validation with max length constraints
        builder.Property(j => j.Name)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(j => j.Slug)
               .IsRequired()
               .HasMaxLength(500);

        // Enum validation
        builder.Property(j => j.RemoteType)
               .IsRequired();

        builder.Property(j => j.Site)
               .IsRequired();

        // Numeric validation - RemotePercent should be 0-100 if provided
        builder.Property(j => j.RemotePercent)
               .HasColumnType("int");

        // Add index on Slug for better query performance
        builder.HasIndex(j => j.Slug);

        // Add composite index on Site and Slug for uniqueness check
        builder.HasIndex(j => new { j.Site, j.Slug })
               .IsUnique();

        builder.HasOne(j => j.CompanyName)
               .WithOne(cn => cn.JobAd)
               .HasForeignKey<CompanyName>(cn => cn.JobAdId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(j => j.Cities)
               .WithMany(c => c.JobAds);
    }
}
