using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class CompanyNameConfiguration : IEntityTypeConfiguration<CompanyName>
{
    public void Configure(EntityTypeBuilder<CompanyName> builder)
    {
        builder.HasKey(d => d.Id);

        // String validation with max length constraint
        builder.Property(cn => cn.Name)
               .IsRequired()
               .HasMaxLength(255);

        // Add index on Name for better query performance
        builder.HasIndex(cn => cn.Name);
    }
}
