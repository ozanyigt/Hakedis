using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class HakedisPeriodConfiguration : IEntityTypeConfiguration<HakedisPeriod>
{
    public void Configure(EntityTypeBuilder<HakedisPeriod> builder)
    {
        builder.ToTable("HakedisPeriods").HasKey(hp => hp.Id);

        builder.Property(hp => hp.Id).HasColumnName("Id").IsRequired();
        builder.Property(hp => hp.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(hp => hp.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(hp => hp.PeriodNumber).HasColumnName("PeriodNumber").IsRequired();
        builder.Property(hp => hp.Name).HasColumnName("Name").IsRequired();
        builder.Property(hp => hp.PeriodStart).HasColumnName("PeriodStart").IsRequired();
        builder.Property(hp => hp.PeriodEnd).HasColumnName("PeriodEnd").IsRequired();
        builder.Property(hp => hp.Status).HasColumnName("Status").IsRequired();
        builder.Property(hp => hp.TotalAmount).HasColumnName("TotalAmount").IsRequired().HasPrecision(18, 2);
        builder.Property(hp => hp.DeductionAmount).HasColumnName("DeductionAmount").IsRequired().HasPrecision(18, 2);
        builder.Property(hp => hp.NetAmount).HasColumnName("NetAmount").IsRequired().HasPrecision(18, 2);
        builder.Property(hp => hp.ApprovedByUserId).HasColumnName("ApprovedByUserId");
        builder.Property(hp => hp.ApprovedAt).HasColumnName("ApprovedAt");
        builder.Property(hp => hp.Notes).HasColumnName("Notes");
        builder.Property(hp => hp.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(hp => hp.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(hp => hp.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(hp => !hp.DeletedDate.HasValue);

        builder.HasOne(hp => hp.Tenant).WithMany().HasForeignKey(hp => hp.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(hp => hp.Project).WithMany(p => p.HakedisPeriods).HasForeignKey(hp => hp.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(hp => hp.ApprovedByUser).WithMany(u => u.ApprovedHakedisPeriods).HasForeignKey(hp => hp.ApprovedByUserId).OnDelete(DeleteBehavior.NoAction);
    }
}