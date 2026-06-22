using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans").HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id).HasColumnName("Id").IsRequired();
        builder.Property(sp => sp.Code).HasColumnName("Code").IsRequired();
        builder.Property(sp => sp.Name).HasColumnName("Name").IsRequired();
        builder.Property(sp => sp.Description).HasColumnName("Description");
        builder.Property(sp => sp.MonthlyPrice).HasColumnName("MonthlyPrice").IsRequired().HasPrecision(18, 2);
        builder.Property(sp => sp.YearlyPrice).HasColumnName("YearlyPrice").IsRequired().HasPrecision(18, 2);
        builder.Property(sp => sp.EnabledModules).HasColumnName("EnabledModules").IsRequired();
        builder.Property(sp => sp.MaxSiteCount).HasColumnName("MaxSiteCount").IsRequired();
        builder.Property(sp => sp.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(sp => sp.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(sp => sp.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(sp => sp.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(sp => !sp.DeletedDate.HasValue);
    }
}