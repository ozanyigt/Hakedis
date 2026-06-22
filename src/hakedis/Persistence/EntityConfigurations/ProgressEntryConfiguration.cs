using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ProgressEntryConfiguration : IEntityTypeConfiguration<ProgressEntry>
{
    public void Configure(EntityTypeBuilder<ProgressEntry> builder)
    {
        builder.ToTable("ProgressEntries").HasKey(pe => pe.Id);

        builder.Property(pe => pe.Id).HasColumnName("Id").IsRequired();
        builder.Property(pe => pe.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(pe => pe.HakedisPeriodId).HasColumnName("HakedisPeriodId").IsRequired();
        builder.Property(pe => pe.ContractItemId).HasColumnName("ContractItemId").IsRequired();
        builder.Property(pe => pe.QuantityThisPeriod).HasColumnName("QuantityThisPeriod").IsRequired().HasPrecision(18, 4);
        builder.Property(pe => pe.CumulativeQuantity).HasColumnName("CumulativeQuantity").IsRequired().HasPrecision(18, 4);
        builder.Property(pe => pe.AmountThisPeriod).HasColumnName("AmountThisPeriod").IsRequired().HasPrecision(18, 2);
        builder.Property(pe => pe.MetrajResultId).HasColumnName("MetrajResultId");
        builder.Property(pe => pe.IsManualEntry).HasColumnName("IsManualEntry").IsRequired();
        builder.Property(pe => pe.Notes).HasColumnName("Notes");
        builder.Property(pe => pe.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(pe => pe.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(pe => pe.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(pe => !pe.DeletedDate.HasValue);

        builder.HasOne(pe => pe.Tenant).WithMany().HasForeignKey(pe => pe.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pe => pe.HakedisPeriod).WithMany(hp => hp.ProgressEntries).HasForeignKey(pe => pe.HakedisPeriodId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pe => pe.ContractItem).WithMany(ci => ci.ProgressEntries).HasForeignKey(pe => pe.ContractItemId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pe => pe.MetrajResult).WithMany(mr => mr.ProgressEntries).HasForeignKey(pe => pe.MetrajResultId).OnDelete(DeleteBehavior.NoAction);
    }
}