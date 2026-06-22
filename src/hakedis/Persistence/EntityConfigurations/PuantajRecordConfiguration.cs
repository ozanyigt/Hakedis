using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class PuantajRecordConfiguration : IEntityTypeConfiguration<PuantajRecord>
{
    public void Configure(EntityTypeBuilder<PuantajRecord> builder)
    {
        builder.ToTable("PuantajRecords").HasKey(pr => pr.Id);

        builder.Property(pr => pr.Id).HasColumnName("Id").IsRequired();
        builder.Property(pr => pr.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(pr => pr.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(pr => pr.SiteId).HasColumnName("SiteId");
        builder.Property(pr => pr.WorkerId).HasColumnName("WorkerId");
        builder.Property(pr => pr.WorkDate).HasColumnName("WorkDate").IsRequired();
        builder.Property(pr => pr.WorkType).HasColumnName("WorkType").IsRequired();
        builder.Property(pr => pr.DayCount).HasColumnName("DayCount").IsRequired().HasPrecision(18, 4);
        builder.Property(pr => pr.OvertimeHours).HasColumnName("OvertimeHours").IsRequired().HasPrecision(18, 4);
        builder.Property(pr => pr.Status).HasColumnName("Status").IsRequired();
        builder.Property(pr => pr.ApprovedByUserId).HasColumnName("ApprovedByUserId");
        builder.Property(pr => pr.ApprovedAt).HasColumnName("ApprovedAt");
        builder.Property(pr => pr.Notes).HasColumnName("Notes");
        builder.Property(pr => pr.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(pr => pr.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(pr => pr.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(pr => !pr.DeletedDate.HasValue);

        builder.HasOne(pr => pr.Tenant).WithMany().HasForeignKey(pr => pr.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pr => pr.Project).WithMany(p => p.PuantajRecords).HasForeignKey(pr => pr.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pr => pr.Site).WithMany(s => s.PuantajRecords).HasForeignKey(pr => pr.SiteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pr => pr.Worker).WithMany(w => w.PuantajRecords).HasForeignKey(pr => pr.WorkerId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pr => pr.ApprovedByUser).WithMany(u => u.ApprovedPuantajRecords).HasForeignKey(pr => pr.ApprovedByUserId).OnDelete(DeleteBehavior.NoAction);
    }
}