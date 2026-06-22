using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class DrawingConfiguration : IEntityTypeConfiguration<Drawing>
{
    public void Configure(EntityTypeBuilder<Drawing> builder)
    {
        builder.ToTable("Drawings").HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("Id").IsRequired();
        builder.Property(d => d.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(d => d.ProjectId).HasColumnName("ProjectId").IsRequired();
        builder.Property(d => d.SiteId).HasColumnName("SiteId");
        builder.Property(d => d.FileName).HasColumnName("FileName").IsRequired();
        builder.Property(d => d.FilePath).HasColumnName("FilePath").IsRequired();
        builder.Property(d => d.FileExtension).HasColumnName("FileExtension").IsRequired();
        builder.Property(d => d.FileSizeBytes).HasColumnName("FileSizeBytes").IsRequired();
        builder.Property(d => d.Status).HasColumnName("Status").IsRequired();
        builder.Property(d => d.ParseErrorMessage).HasColumnName("ParseErrorMessage");
        builder.Property(d => d.ParsedAt).HasColumnName("ParsedAt");
        builder.Property(d => d.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(d => d.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(d => d.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(d => !d.DeletedDate.HasValue);

        builder.HasOne(d => d.Tenant).WithMany().HasForeignKey(d => d.TenantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(d => d.Project).WithMany(p => p.Drawings).HasForeignKey(d => d.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(d => d.Site).WithMany(s => s.Drawings).HasForeignKey(d => d.SiteId).OnDelete(DeleteBehavior.NoAction);
    }
}