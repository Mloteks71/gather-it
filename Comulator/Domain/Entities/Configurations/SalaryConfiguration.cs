using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;
public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasOne(builder => builder.JobAd)
            .WithMany(jobAd => jobAd.Salaries)
            .HasForeignKey(salary => salary.JobAdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
