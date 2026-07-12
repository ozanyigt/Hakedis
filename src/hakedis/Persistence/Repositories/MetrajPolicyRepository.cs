using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class MetrajPolicyRepository : EfRepositoryBase<MetrajPolicy, Guid, BaseDbContext>, IMetrajPolicyRepository
{
    public MetrajPolicyRepository(BaseDbContext context)
        : base(context) { }
}
