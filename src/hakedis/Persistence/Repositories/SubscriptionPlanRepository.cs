using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class SubscriptionPlanRepository : EfRepositoryBase<SubscriptionPlan, Guid, BaseDbContext>, ISubscriptionPlanRepository
{
    public SubscriptionPlanRepository(BaseDbContext context) : base(context)
    {
    }
}