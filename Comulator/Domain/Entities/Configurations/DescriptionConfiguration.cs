using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class DescriptionConfiguration : IEntityTypeConfiguration<Description>
{
    public void Configure(EntityTypeBuilder<Description> builder)
    {
        builder.HasKey(d => d.JobAdId);

        builder.Property(d => d.JobAdId)
            .ValueGeneratedNever();
    }
}
