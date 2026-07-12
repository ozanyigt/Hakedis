using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MetrajPolicyConfiguration : IEntityTypeConfiguration<MetrajPolicy>
{
    public void Configure(EntityTypeBuilder<MetrajPolicy> builder)
    {
        builder.ToTable("MetrajPolicies").HasKey(item => item.Id);

        builder.Property(item => item.Id).HasColumnName("Id").IsRequired();
        builder.Property(item => item.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(item => item.Code).HasColumnName("Code").HasMaxLength(50).IsRequired();
        builder.Property(item => item.Title).HasColumnName("Title").HasMaxLength(200).IsRequired();
        builder.Property(item => item.Body).HasColumnName("Body").IsRequired();
        builder.Property(item => item.Version).HasColumnName("Version").IsRequired();
        builder.Property(item => item.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(item => item.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(item => item.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(item => item.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(item => new { item.TenantId, item.Code }).IsUnique();
        builder.HasQueryFilter(item => !item.DeletedDate.HasValue);

        builder
            .HasOne(item => item.Tenant)
            .WithMany()
            .HasForeignKey(item => item.TenantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
