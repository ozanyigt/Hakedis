using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Tenants;

public interface ITenantService
{
    Task<Tenant?> GetAsync(
        Expression<Func<Tenant, bool>> predicate,
        Func<IQueryable<Tenant>, IIncludableQueryable<Tenant, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Tenant>?> GetListAsync(
        Expression<Func<Tenant, bool>>? predicate = null,
        Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>>? orderBy = null,
        Func<IQueryable<Tenant>, IIncludableQueryable<Tenant, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Tenant> AddAsync(Tenant tenant);
    Task<Tenant> UpdateAsync(Tenant tenant);
    Task<Tenant> DeleteAsync(Tenant tenant, bool permanent = false);
}
