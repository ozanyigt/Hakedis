using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MetrajRuleTemplateConfiguration : IEntityTypeConfiguration<MetrajRuleTemplate>
{
    public void Configure(EntityTypeBuilder<MetrajRuleTemplate> builder)
    {
        builder.ToTable("MetrajRuleTemplates").HasKey(mrt => mrt.Id);

        builder.Property(mrt => mrt.Id).HasColumnName("Id").IsRequired();
        builder.Property(mrt => mrt.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(mrt => mrt.Name).HasColumnName("Name").IsRequired();
        builder.Property(mrt => mrt.KalemType).HasColumnName("KalemType").IsRequired();
        builder.Property(mrt => mrt.LayerPatterns).HasColumnName("LayerPatterns").IsRequired();
        builder.Property(mrt => mrt.EntityTypes).HasColumnName("EntityTypes").IsRequired();
        builder.Property(mrt => mrt.Unit).HasColumnName("Unit").IsRequired();
        builder.Property(mrt => mrt.DefaultThickness).HasColumnName("DefaultThickness").HasPrecision(18, 4);
        builder.Property(mrt => mrt.DefaultHeight).HasColumnName("DefaultHeight").HasPrecision(18, 4);
        builder.Property(mrt => mrt.DeductOpenings).HasColumnName("DeductOpenings").IsRequired();
        builder.Property(mrt => mrt.OpeningLayerPatterns).HasColumnName("OpeningLayerPatterns");
        builder.Property(mrt => mrt.IsDefault).HasColumnName("IsDefault").IsRequired();
        builder.Property(mrt => mrt.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(mrt => mrt.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(mrt => mrt.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(mrt => mrt.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(mrt => !mrt.DeletedDate.HasValue);

        builder.HasOne(mrt => mrt.Tenant).WithMany().HasForeignKey(mrt => mrt.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}