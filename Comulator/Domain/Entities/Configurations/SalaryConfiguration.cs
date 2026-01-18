using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.HasKey(d => d.Id);

        // Enum validation
        builder.Property(s => s.ContractType)
               .IsRequired();

        // Numeric validation - salary values should be non-negative
        builder.Property(s => s.SalaryMin)
               .IsRequired()
               .HasColumnType("int");

        builder.Property(s => s.SalaryMax)
               .IsRequired()
               .HasColumnType("int");

        builder.HasOne(builder => builder.JobAd)
            .WithMany(jobAd => jobAd.Salaries)
            .HasForeignKey(salary => salary.JobAdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
