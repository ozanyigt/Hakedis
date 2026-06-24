using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class HakedisDeductionLineConfiguration : IEntityTypeConfiguration<HakedisDeductionLine>
{
    public void Configure(EntityTypeBuilder<HakedisDeductionLine> builder)
    {
        builder.ToTable("HakedisDeductionLines").HasKey(line => line.Id);

        builder.Property(line => line.Id).HasColumnName("Id").IsRequired();
        builder.Property(line => line.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(line => line.HakedisPeriodId).HasColumnName("HakedisPeriodId").IsRequired();
        builder.Property(line => line.Category).HasColumnName("Category").IsRequired();
        builder.Property(line => line.Description).HasColumnName("Description").IsRequired().HasMaxLength(256);
        builder.Property(line => line.Amount).HasColumnName("Amount").IsRequired().HasPrecision(18, 2);
        builder.Property(line => line.Notes).HasColumnName("Notes");
        builder.Property(line => line.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(line => line.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(line => line.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(line => !line.DeletedDate.HasValue);

        builder.HasOne(line => line.Tenant).WithMany().HasForeignKey(line => line.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(line => line.HakedisPeriod)
            .WithMany(period => period.DeductionLines)
            .HasForeignKey(line => line.HakedisPeriodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
