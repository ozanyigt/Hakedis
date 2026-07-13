using Application.Services.CurrentUser;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Application.Features.Inventory;

public class InventoryBusinessRules(
    IMaterialRepository materialRepository,
    IProjectRepository projectRepository,
    ISiteRepository siteRepository,
    ICurrentUserService currentUserService) : BaseBusinessRules
{
    public async Task<Guid?> ResolveTenantAsync(
        Guid? requestedTenantId, bool required, CancellationToken cancellationToken)
    {
        if (currentUserService.IsGlobalAdmin)
        {
            if (required && !requestedTenantId.HasValue)
                throw new BusinessException("TenantId is required for a global administrator.");
            return requestedTenantId;
        }
        User? user = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (user?.TenantId is null)
            throw new BusinessException("The current user is not assigned to a tenant.");
        if (requestedTenantId.HasValue && requestedTenantId != user.TenantId)
            throw new BusinessException("The requested tenant is outside the current user's tenant.");
        return user.TenantId;
    }

    public Guid UserId =>
        currentUserService.UserId ?? throw new BusinessException("An authenticated user is required.");

    public async Task ValidateSiteAsync(
        Guid tenantId, Guid siteId, Guid? projectId, CancellationToken cancellationToken)
    {
        Site? site = await siteRepository.GetAsync(
            x => x.Id == siteId && x.TenantId == tenantId
                 && (!projectId.HasValue || x.ProjectId == projectId.Value),
            enableTracking: false, cancellationToken: cancellationToken);
        if (site is null)
            throw new BusinessException("Site does not belong to the selected tenant and project.");
        if (projectId.HasValue && !await projectRepository.AnyAsync(
                x => x.Id == projectId && x.TenantId == tenantId,
                cancellationToken: cancellationToken))
            throw new BusinessException("Project does not belong to the selected tenant.");
    }

    public async Task<Material> GetMaterialAsync(
        Guid id, Guid? requestedTenantId, CancellationToken cancellationToken)
    {
        Guid? tenantId = await ResolveTenantAsync(requestedTenantId, false, cancellationToken);
        Material? material = await materialRepository.GetAsync(
            x => x.Id == id && (!tenantId.HasValue || x.TenantId == tenantId.Value),
            cancellationToken: cancellationToken);
        return material ?? throw new BusinessException("Material does not exist.");
    }
}
