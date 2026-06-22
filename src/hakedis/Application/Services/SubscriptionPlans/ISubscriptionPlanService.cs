using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.SubscriptionPlans;

public interface ISubscriptionPlanService
{
    Task<SubscriptionPlan?> GetAsync(
        Expression<Func<SubscriptionPlan, bool>> predicate,
        Func<IQueryable<SubscriptionPlan>, IIncludableQueryable<SubscriptionPlan, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<SubscriptionPlan>?> GetListAsync(
        Expression<Func<SubscriptionPlan, bool>>? predicate = null,
        Func<IQueryable<SubscriptionPlan>, IOrderedQueryable<SubscriptionPlan>>? orderBy = null,
        Func<IQueryable<SubscriptionPlan>, IIncludableQueryable<SubscriptionPlan, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan);
    Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan);
    Task<SubscriptionPlan> DeleteAsync(SubscriptionPlan subscriptionPlan, bool permanent = false);
}
