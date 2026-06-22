using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MetrajResultConfiguration : IEntityTypeConfiguration<MetrajResult>
{
    public void Configure(EntityTypeBuilder<MetrajResult> builder)
    {
        builder.ToTable("MetrajResults").HasKey(mr => mr.Id);

        builder.Property(mr => mr.Id).HasColumnName("Id").IsRequired();
        builder.Property(mr => mr.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(mr => mr.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(mr => mr.SiteId).HasColumnName("SiteId");
        builder.Property(mr => mr.DrawingId).HasColumnName("DrawingId").IsRequired();
        builder.Property(mr => mr.KalemType).HasColumnName("KalemType").IsRequired();
        builder.Property(mr => mr.Unit).HasColumnName("Unit").IsRequired();
        builder.Property(mr => mr.Quantity).HasColumnName("Quantity").IsRequired().HasPrecision(18, 4);
        builder.Property(mr => mr.FloorName).HasColumnName("FloorName");
        builder.Property(mr => mr.SpaceName).HasColumnName("SpaceName");
        builder.Property(mr => mr.CalculatedAt).HasColumnName("CalculatedAt").IsRequired();
        builder.Property(mr => mr.Notes).HasColumnName("Notes");
        builder.Property(mr => mr.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(mr => mr.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(mr => mr.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(mr => !mr.DeletedDate.HasValue);

        builder.HasOne(mr => mr.Tenant).WithMany().HasForeignKey(mr => mr.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(mr => mr.Project).WithMany(p => p.MetrajResults).HasForeignKey(mr => mr.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(mr => mr.Site).WithMany(s => s.MetrajResults).HasForeignKey(mr => mr.SiteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(mr => mr.Drawing).WithMany(d => d.MetrajResults).HasForeignKey(mr => mr.DrawingId).OnDelete(DeleteBehavior.NoAction);
    }
}