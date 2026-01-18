using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(d => d.Id);

        // String validation with max length constraint
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Add unique index on Name to prevent duplicate cities
        builder.HasIndex(c => c.Name)
               .IsUnique();
    }
}
