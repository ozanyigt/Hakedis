using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class DailySiteReportConfiguration : IEntityTypeConfiguration<DailySiteReport>
{
    public void Configure(EntityTypeBuilder<DailySiteReport> builder)
    {
        builder.ToTable("DailySiteReports").HasKey(x => x.Id);
        builder.Property(x => x.ReportDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.Weather).IsRequired();
        builder.Property(x => x.MinTemperatureCelsius).HasPrecision(5, 2);
        builder.Property(x => x.MaxTemperatureCelsius).HasPrecision(5, 2);
        builder.Property(x => x.WorkSummary).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.WorkforceNotes).HasMaxLength(4000);
        builder.Property(x => x.EquipmentNotes).HasMaxLength(4000);
        builder.Property(x => x.MaterialNotes).HasMaxLength(4000);
        builder.Property(x => x.BlockersNotes).HasMaxLength(4000);
        builder.Property(x => x.Notes).HasMaxLength(4000);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.SiteId, x.ReportDate })
            .IsUnique()
            .HasFilter("[DeletedDate] IS NULL")
            .HasDatabaseName("UX_DailySiteReports_Tenant_Site_Date_Active");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Project).WithMany(x => x.DailySiteReports).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Site).WithMany(x => x.DailySiteReports).HasForeignKey(x => x.SiteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.CreatedByUser).WithMany(x => x.CreatedDailySiteReports).HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.ApprovedByUser).WithMany(x => x.ApprovedDailySiteReports).HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.Photos).WithOne(x => x.DailySiteReport).HasForeignKey(x => x.DailySiteReportId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DailySiteReportPhotoConfiguration : IEntityTypeConfiguration<DailySiteReportPhoto>
{
    public void Configure(EntityTypeBuilder<DailySiteReportPhoto> builder)
    {
        builder.ToTable("DailySiteReportPhotos").HasKey(x => x.Id);
        builder.Property(x => x.Url).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SizeBytes).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SortOrder).IsRequired();
        builder.HasIndex(x => x.DailySiteReportId);
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}
