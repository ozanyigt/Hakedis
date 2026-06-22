using Application.Features.Auth.Constants;
using Application.Features.OperationClaims.Constants;
using Application.Features.UserOperationClaims.Constants;
using Application.Features.Users.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NArchitecture.Core.Security.Constants;
using Application.Features.Tenants.Constants;
using Application.Features.SubscriptionPlans.Constants;
using Application.Features.Subscriptions.Constants;
using Application.Features.Projects.Constants;
using Application.Features.Sites.Constants;
using Application.Features.Drawings.Constants;
using Application.Features.MetrajRuleTemplates.Constants;
using Application.Features.MetrajResults.Constants;
using Application.Features.Workers.Constants;
using Application.Features.PuantajRecords.Constants;
using Application.Features.ContractItems.Constants;
using Application.Features.HakedisPeriods.Constants;
using Application.Features.ProgressEntries.Constants;














namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

        builder.Property(oc => oc.Id).HasColumnName("Id").IsRequired();
        builder.Property(oc => oc.Name).HasColumnName("Name").IsRequired();
        builder.Property(oc => oc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oc => oc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(oc => oc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(oc => !oc.DeletedDate.HasValue);

        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int AdminId => 1;
    private IEnumerable<OperationClaim> _seeds
    {
        get
        {
            yield return new() { Id = AdminId, Name = GeneralOperationClaims.Admin };

            IEnumerable<OperationClaim> featureOperationClaims = getFeatureOperationClaims(AdminId);
            foreach (OperationClaim claim in featureOperationClaims)
                yield return claim;
        }
    }

#pragma warning disable S1854 // Unused assignments should be removed
    private IEnumerable<OperationClaim> getFeatureOperationClaims(int initialId)
    {
        int lastId = initialId;
        List<OperationClaim> featureOperationClaims = new();

        #region Auth
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = AuthOperationClaims.Admin },
                new() { Id = ++lastId, Name = AuthOperationClaims.Read },
                new() { Id = ++lastId, Name = AuthOperationClaims.Write },
                new() { Id = ++lastId, Name = AuthOperationClaims.RevokeToken },
            ]
        );
        #endregion

        #region OperationClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Admin },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Read },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Write },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Create },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Update },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Delete },
            ]
        );
        #endregion

        #region UserOperationClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Admin },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Read },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Write },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Create },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Update },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Delete },
            ]
        );
        #endregion

        #region Users
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UsersOperationClaims.Admin },
                new() { Id = ++lastId, Name = UsersOperationClaims.Read },
                new() { Id = ++lastId, Name = UsersOperationClaims.Write },
                new() { Id = ++lastId, Name = UsersOperationClaims.Create },
                new() { Id = ++lastId, Name = UsersOperationClaims.Update },
                new() { Id = ++lastId, Name = UsersOperationClaims.Delete },
            ]
        );
        #endregion

        
        #region Tenants CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Tenants.Admin" },
                new() { Id = ++lastId, Name = "Tenants.Read" },
                new() { Id = ++lastId, Name = "Tenants.Write" },
                
                new() { Id = ++lastId, Name = "Tenants.Create" },
                new() { Id = ++lastId, Name = "Tenants.Update" },
                new() { Id = ++lastId, Name = "Tenants.Delete" },
            ]
        );
        #endregion
        
        
        #region SubscriptionPlans CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "SubscriptionPlans.Admin" },
                new() { Id = ++lastId, Name = "SubscriptionPlans.Read" },
                new() { Id = ++lastId, Name = "SubscriptionPlans.Write" },
                
                new() { Id = ++lastId, Name = "SubscriptionPlans.Create" },
                new() { Id = ++lastId, Name = "SubscriptionPlans.Update" },
                new() { Id = ++lastId, Name = "SubscriptionPlans.Delete" },
            ]
        );
        #endregion
        
        
        #region Subscriptions CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Subscriptions.Admin" },
                new() { Id = ++lastId, Name = "Subscriptions.Read" },
                new() { Id = ++lastId, Name = "Subscriptions.Write" },
                
                new() { Id = ++lastId, Name = "Subscriptions.Create" },
                new() { Id = ++lastId, Name = "Subscriptions.Update" },
                new() { Id = ++lastId, Name = "Subscriptions.Delete" },
            ]
        );
        #endregion
        
        
        #region Projects CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Projects.Admin" },
                new() { Id = ++lastId, Name = "Projects.Read" },
                new() { Id = ++lastId, Name = "Projects.Write" },
                
                new() { Id = ++lastId, Name = "Projects.Create" },
                new() { Id = ++lastId, Name = "Projects.Update" },
                new() { Id = ++lastId, Name = "Projects.Delete" },
            ]
        );
        #endregion
        
        
        #region Sites CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Sites.Admin" },
                new() { Id = ++lastId, Name = "Sites.Read" },
                new() { Id = ++lastId, Name = "Sites.Write" },
                
                new() { Id = ++lastId, Name = "Sites.Create" },
                new() { Id = ++lastId, Name = "Sites.Update" },
                new() { Id = ++lastId, Name = "Sites.Delete" },
            ]
        );
        #endregion
        
        
        #region Drawings CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Drawings.Admin" },
                new() { Id = ++lastId, Name = "Drawings.Read" },
                new() { Id = ++lastId, Name = "Drawings.Write" },
                
                new() { Id = ++lastId, Name = "Drawings.Create" },
                new() { Id = ++lastId, Name = "Drawings.Update" },
                new() { Id = ++lastId, Name = "Drawings.Delete" },
            ]
        );
        #endregion
        
        
        #region MetrajRuleTemplates CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Admin" },
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Read" },
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Write" },
                
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Create" },
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Update" },
                new() { Id = ++lastId, Name = "MetrajRuleTemplates.Delete" },
            ]
        );
        #endregion
        
        
        #region MetrajResults CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "MetrajResults.Admin" },
                new() { Id = ++lastId, Name = "MetrajResults.Read" },
                new() { Id = ++lastId, Name = "MetrajResults.Write" },
                
                new() { Id = ++lastId, Name = "MetrajResults.Create" },
                new() { Id = ++lastId, Name = "MetrajResults.Update" },
                new() { Id = ++lastId, Name = "MetrajResults.Delete" },
            ]
        );
        #endregion
        
        
        #region Workers CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "Workers.Admin" },
                new() { Id = ++lastId, Name = "Workers.Read" },
                new() { Id = ++lastId, Name = "Workers.Write" },
                
                new() { Id = ++lastId, Name = "Workers.Create" },
                new() { Id = ++lastId, Name = "Workers.Update" },
                new() { Id = ++lastId, Name = "Workers.Delete" },
            ]
        );
        #endregion
        
        
        #region PuantajRecords CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "PuantajRecords.Admin" },
                new() { Id = ++lastId, Name = "PuantajRecords.Read" },
                new() { Id = ++lastId, Name = "PuantajRecords.Write" },
                
                new() { Id = ++lastId, Name = "PuantajRecords.Create" },
                new() { Id = ++lastId, Name = "PuantajRecords.Update" },
                new() { Id = ++lastId, Name = "PuantajRecords.Delete" },
            ]
        );
        #endregion
        
        
        #region ContractItems CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "ContractItems.Admin" },
                new() { Id = ++lastId, Name = "ContractItems.Read" },
                new() { Id = ++lastId, Name = "ContractItems.Write" },
                
                new() { Id = ++lastId, Name = "ContractItems.Create" },
                new() { Id = ++lastId, Name = "ContractItems.Update" },
                new() { Id = ++lastId, Name = "ContractItems.Delete" },
            ]
        );
        #endregion
        
        
        #region HakedisPeriods CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "HakedisPeriods.Admin" },
                new() { Id = ++lastId, Name = "HakedisPeriods.Read" },
                new() { Id = ++lastId, Name = "HakedisPeriods.Write" },
                
                new() { Id = ++lastId, Name = "HakedisPeriods.Create" },
                new() { Id = ++lastId, Name = "HakedisPeriods.Update" },
                new() { Id = ++lastId, Name = "HakedisPeriods.Delete" },
            ]
        );
        #endregion
        
        
        #region ProgressEntries CRUD
        
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = "ProgressEntries.Admin" },
                new() { Id = ++lastId, Name = "ProgressEntries.Read" },
                new() { Id = ++lastId, Name = "ProgressEntries.Write" },
                
                new() { Id = ++lastId, Name = "ProgressEntries.Create" },
                new() { Id = ++lastId, Name = "ProgressEntries.Update" },
                new() { Id = ++lastId, Name = "ProgressEntries.Delete" },
            ]
        );
        #endregion
        
        return featureOperationClaims;
    }
#pragma warning restore S1854 // Unused assignments should be removed
}
