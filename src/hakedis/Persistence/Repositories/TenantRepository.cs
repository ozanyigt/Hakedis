using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class TenantRepository : EfRepositoryBase<Tenant, Guid, BaseDbContext>, ITenantRepository
{
    public TenantRepository(BaseDbContext context) : base(context)
    {
    }
}