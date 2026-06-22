using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ITenantRepository : IAsyncRepository<Tenant, Guid>, IRepository<Tenant, Guid>
{
}