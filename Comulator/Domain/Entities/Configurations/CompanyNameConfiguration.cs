using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class CompanyNameConfiguration : IEntityTypeConfiguration<CompanyName>
{
    public void Configure(EntityTypeBuilder<CompanyName> builder)
    {
        builder.HasKey(d => d.Id);
    }
}
