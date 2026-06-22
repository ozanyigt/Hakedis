using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions").HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("Id").IsRequired();
        builder.Property(s => s.TenantId).HasColumnName("TenantId").IsRequired();
        builder.Property(s => s.SubscriptionPlanId).HasColumnName("SubscriptionPlanId").IsRequired();
        builder.Property(s => s.BillingCycle).HasColumnName("BillingCycle").IsRequired();
        builder.Property(s => s.Status).HasColumnName("Status").IsRequired();
        builder.Property(s => s.StartDate).HasColumnName("StartDate").IsRequired();
        builder.Property(s => s.EndDate).HasColumnName("EndDate");
        builder.Property(s => s.IsManualAssignment).HasColumnName("IsManualAssignment").IsRequired();
        builder.Property(s => s.Notes).HasColumnName("Notes");
        builder.Property(s => s.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(s => s.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(s => !s.DeletedDate.HasValue);

        builder.HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.SubscriptionPlan).WithMany(sp => sp.Subscriptions).HasForeignKey(s => s.SubscriptionPlanId).OnDelete(DeleteBehavior.NoAction);
    }
}