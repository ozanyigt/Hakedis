using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.ToTable("Workers").HasKey(w => w.Id);

        builder.Property(w => w.Id).HasColumnName("Id").IsRequired();
        builder.Property(w => w.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(w => w.FullName).HasColumnName("FullName").IsRequired();
        builder.Property(w => w.Trade).HasColumnName("Trade");
        builder.Property(w => w.Phone).HasColumnName("Phone");
        builder.Property(w => w.IdentityNumber).HasColumnName("IdentityNumber");
        builder.Property(w => w.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(w => w.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(w => w.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(w => w.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(w => !w.DeletedDate.HasValue);

        builder.HasOne(w => w.Tenant).WithMany().HasForeignKey(w => w.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}