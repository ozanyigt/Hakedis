using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ProjectMetrajLayerMappingConfiguration : IEntityTypeConfiguration<ProjectMetrajLayerMapping>
{
    public void Configure(EntityTypeBuilder<ProjectMetrajLayerMapping> builder)
    {
        builder.ToTable("ProjectMetrajLayerMappings").HasKey(item => item.Id);

        builder.Property(item => item.Id).HasColumnName("Id").IsRequired();
        builder.Property(item => item.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(item => item.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(item => item.KalemType).HasColumnName("KalemType").IsRequired();
        builder.Property(item => item.LayerNames).HasColumnName("LayerNames").IsRequired();
        builder.Property(item => item.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(item => item.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(item => item.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(item => new { item.ProjectId, item.KalemType }).IsUnique();
        builder.HasQueryFilter(item => !item.DeletedDate.HasValue);

        builder
            .HasOne(item => item.Tenant)
            .WithMany()
            .HasForeignKey(item => item.TenantId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(item => item.Project)
            .WithMany(project => project.MetrajLayerMappings)
            .HasForeignKey(item => item.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
