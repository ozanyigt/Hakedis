using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ISubscriptionRepository : IAsyncRepository<Subscription, Guid>, IRepository<Subscription, Guid>
{
}