using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ISubscriptionPlanRepository : IAsyncRepository<SubscriptionPlan, Guid>, IRepository<SubscriptionPlan, Guid>
{
}