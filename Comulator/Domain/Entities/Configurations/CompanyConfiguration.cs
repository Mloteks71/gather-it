using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasMany(c => c.Names)
            .WithOne(x => x.Company)
            .HasForeignKey(n => n.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
