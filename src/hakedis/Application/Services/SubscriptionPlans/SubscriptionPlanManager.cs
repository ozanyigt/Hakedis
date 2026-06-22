using Application.Features.SubscriptionPlans.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.SubscriptionPlans;

public class SubscriptionPlanManager : ISubscriptionPlanService
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly SubscriptionPlanBusinessRules _subscriptionPlanBusinessRules;

    public SubscriptionPlanManager(ISubscriptionPlanRepository subscriptionPlanRepository, SubscriptionPlanBusinessRules subscriptionPlanBusinessRules)
    {
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _subscriptionPlanBusinessRules = subscriptionPlanBusinessRules;
    }

    public async Task<SubscriptionPlan?> GetAsync(
        Expression<Func<SubscriptionPlan, bool>> predicate,
        Func<IQueryable<SubscriptionPlan>, IIncludableQueryable<SubscriptionPlan, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return subscriptionPlan;
    }

    public async Task<IPaginate<SubscriptionPlan>?> GetListAsync(
        Expression<Func<SubscriptionPlan, bool>>? predicate = null,
        Func<IQueryable<SubscriptionPlan>, IOrderedQueryable<SubscriptionPlan>>? orderBy = null,
        Func<IQueryable<SubscriptionPlan>, IIncludableQueryable<SubscriptionPlan, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<SubscriptionPlan> subscriptionPlanList = await _subscriptionPlanRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return subscriptionPlanList;
    }

    public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan)
    {
        SubscriptionPlan addedSubscriptionPlan = await _subscriptionPlanRepository.AddAsync(subscriptionPlan);

        return addedSubscriptionPlan;
    }

    public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan)
    {
        SubscriptionPlan updatedSubscriptionPlan = await _subscriptionPlanRepository.UpdateAsync(subscriptionPlan);

        return updatedSubscriptionPlan;
    }

    public async Task<SubscriptionPlan> DeleteAsync(SubscriptionPlan subscriptionPlan, bool permanent = false)
    {
        SubscriptionPlan deletedSubscriptionPlan = await _subscriptionPlanRepository.DeleteAsync(subscriptionPlan);

        return deletedSubscriptionPlan;
    }
}
