using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ContractItemConfiguration : IEntityTypeConfiguration<ContractItem>
{
    public void Configure(EntityTypeBuilder<ContractItem> builder)
    {
        builder.ToTable("ContractItems").HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id).HasColumnName("Id").IsRequired();
        builder.Property(ci => ci.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(ci => ci.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(ci => ci.KalemType).HasColumnName("KalemType").IsRequired();
        builder.Property(ci => ci.Description).HasColumnName("Description").IsRequired();
        builder.Property(ci => ci.Unit).HasColumnName("Unit").IsRequired();
        builder.Property(ci => ci.UnitPrice).HasColumnName("UnitPrice").IsRequired().HasPrecision(18, 2);
        builder.Property(ci => ci.ContractQuantity).HasColumnName("ContractQuantity").HasPrecision(18, 4);
        builder.Property(ci => ci.SortOrder).HasColumnName("SortOrder").IsRequired();
        builder.Property(ci => ci.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(ci => ci.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(ci => ci.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(ci => !ci.DeletedDate.HasValue);

        builder.HasOne(ci => ci.Tenant).WithMany().HasForeignKey(ci => ci.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(ci => ci.Project).WithMany(p => p.ContractItems).HasForeignKey(ci => ci.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}