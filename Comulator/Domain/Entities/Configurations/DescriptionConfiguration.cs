using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class DescriptionConfiguration : IEntityTypeConfiguration<Description>
{
    public void Configure(EntityTypeBuilder<Description> builder)
    {
        // Configure Description as having JobAdId as its primary key (1:0..1 relationship)
        builder.HasKey(d => d.JobAdId);

        // JobAdId is not generated - it's a foreign key that must reference an existing JobAd
        builder.Property(d => d.JobAdId)
            .ValueGeneratedNever();

        // Explicitly configure the foreign key relationship
        builder.HasOne(d => d.JobAd)
            .WithOne(j => j.Description)
            .HasForeignKey<Description>(d => d.JobAdId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
