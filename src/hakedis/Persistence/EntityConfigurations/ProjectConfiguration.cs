using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects").HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(p => p.Name).HasColumnName("Name").IsRequired();
        builder.Property(p => p.Code).HasColumnName("Code");
        builder.Property(p => p.Location).HasColumnName("Location");
        builder.Property(p => p.ClientName).HasColumnName("ClientName");
        builder.Property(p => p.ContractAmount).HasColumnName("ContractAmount").IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.StartDate).HasColumnName("StartDate");
        builder.Property(p => p.EndDate).HasColumnName("EndDate");
        builder.Property(p => p.Status).HasColumnName("Status").IsRequired();
        builder.Property(p => p.Description).HasColumnName("Description");
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(p => !p.DeletedDate.HasValue);

        builder.HasOne(p => p.Tenant).WithMany(t => t.Projects).HasForeignKey(p => p.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}