using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class DemoRequestConfiguration : IEntityTypeConfiguration<DemoRequest>
{
    public void Configure(EntityTypeBuilder<DemoRequest> builder)
    {
        builder.ToTable("DemoRequests").HasKey(item => item.Id);

        builder.Property(item => item.Id).HasColumnName("Id").IsRequired();
        builder.Property(item => item.CompanyName).HasColumnName("CompanyName").IsRequired().HasMaxLength(200);
        builder.Property(item => item.ContactName).HasColumnName("ContactName").IsRequired().HasMaxLength(200);
        builder.Property(item => item.Email).HasColumnName("Email").IsRequired().HasMaxLength(256);
        builder.Property(item => item.Phone).HasColumnName("Phone").IsRequired().HasMaxLength(50);
        builder.Property(item => item.Interest).HasColumnName("Interest").IsRequired().HasMaxLength(100);
        builder.Property(item => item.Message).HasColumnName("Message").HasMaxLength(2000);
        builder.Property(item => item.Status).HasColumnName("Status").IsRequired();
        builder.Property(item => item.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(item => item.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(item => item.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(item => !item.DeletedDate.HasValue);
    }
}
