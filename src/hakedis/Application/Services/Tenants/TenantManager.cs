using Application.Features.Tenants.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Tenants;

public class TenantManager : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly TenantBusinessRules _tenantBusinessRules;

    public TenantManager(ITenantRepository tenantRepository, TenantBusinessRules tenantBusinessRules)
    {
        _tenantRepository = tenantRepository;
        _tenantBusinessRules = tenantBusinessRules;
    }

    public async Task<Tenant?> GetAsync(
        Expression<Func<Tenant, bool>> predicate,
        Func<IQueryable<Tenant>, IIncludableQueryable<Tenant, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Tenant? tenant = await _tenantRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return tenant;
    }

    public async Task<IPaginate<Tenant>?> GetListAsync(
        Expression<Func<Tenant, bool>>? predicate = null,
        Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>>? orderBy = null,
        Func<IQueryable<Tenant>, IIncludableQueryable<Tenant, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Tenant> tenantList = await _tenantRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return tenantList;
    }

    public async Task<Tenant> AddAsync(Tenant tenant)
    {
        Tenant addedTenant = await _tenantRepository.AddAsync(tenant);

        return addedTenant;
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant)
    {
        Tenant updatedTenant = await _tenantRepository.UpdateAsync(tenant);

        return updatedTenant;
    }

    public async Task<Tenant> DeleteAsync(Tenant tenant, bool permanent = false)
    {
        Tenant deletedTenant = await _tenantRepository.DeleteAsync(tenant);

        return deletedTenant;
    }
}
