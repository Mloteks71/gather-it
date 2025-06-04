using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class JobAdConfiguration : IEntityTypeConfiguration<JobAd>
{
    public void Configure(EntityTypeBuilder<JobAd> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        builder.HasOne(j => j.Description)
               .WithOne(d => d.JobAd)
               .HasForeignKey<Description>(d => d.JobAdId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.CompanyName)
               .WithOne(cn => cn.JobAd)
               .HasForeignKey<CompanyName>(cn => cn.JobAdId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(j => j.Cities)
               .WithMany(c => c.JobAds);
    }
}
