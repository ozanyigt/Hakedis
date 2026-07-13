using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials").HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Unit).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique().HasFilter("[DeletedDate] IS NULL");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class SiteStockBalanceConfiguration : IEntityTypeConfiguration<SiteStockBalance>
{
    public void Configure(EntityTypeBuilder<SiteStockBalance> builder)
    {
        builder.ToTable("SiteStockBalances").HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.AverageUnitCost).HasPrecision(18, 6);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.TenantId, x.SiteId, x.MaterialId }).IsUnique()
            .HasFilter("[DeletedDate] IS NULL");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.ToTable("StockTransactions").HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 6);
        builder.Property(x => x.TotalCost).HasPrecision(18, 6);
        builder.Property(x => x.BalanceQuantityAfter).HasPrecision(18, 4);
        builder.Property(x => x.AverageUnitCostAfter).HasPrecision(18, 6);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Reference).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.TenantId, x.IdempotencyKey }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.SiteId, x.MaterialId, x.OccurredAt });
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class DailySiteReportMaterialLineConfiguration : IEntityTypeConfiguration<DailySiteReportMaterialLine>
{
    public void Configure(EntityTypeBuilder<DailySiteReportMaterialLine> builder)
    {
        builder.ToTable("DailySiteReportMaterialLines").HasKey(x => x.Id);
        builder.Property(x => x.MaterialCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.MaterialName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Unit).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.PostedUnitCost).HasPrecision(18, 6);
        builder.Property(x => x.PostedTotalCost).HasPrecision(18, 6);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.DailySiteReportId, x.MaterialId }).IsUnique()
            .HasFilter("[DeletedDate] IS NULL");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        builder.HasOne(x => x.DailySiteReport).WithMany(x => x.MaterialLines)
            .HasForeignKey(x => x.DailySiteReportId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class DailySiteReportWorkforceSnapshotConfiguration : IEntityTypeConfiguration<DailySiteReportWorkforceSnapshot>
{
    public void Configure(EntityTypeBuilder<DailySiteReportWorkforceSnapshot> builder)
    {
        builder.ToTable("DailySiteReportWorkforceSnapshots").HasKey(x => x.Id);
        builder.Property(x => x.WorkerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Trade).HasMaxLength(100);
        builder.Property(x => x.DayCount).HasPrecision(18, 4);
        builder.Property(x => x.OvertimeHours).HasPrecision(18, 4);
        builder.HasIndex(x => new { x.DailySiteReportId, x.SourcePuantajRecordId }).IsUnique();
        builder.HasIndex(x => new { x.DailySiteReportId, x.CaptureBatchId });
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        builder.HasOne(x => x.DailySiteReport).WithMany(x => x.WorkforceSnapshots)
            .HasForeignKey(x => x.DailySiteReportId).OnDelete(DeleteBehavior.Cascade);
    }
}
