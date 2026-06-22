using Application.Features.Subscriptions.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Subscriptions;

public class SubscriptionManager : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly SubscriptionBusinessRules _subscriptionBusinessRules;

    public SubscriptionManager(ISubscriptionRepository subscriptionRepository, SubscriptionBusinessRules subscriptionBusinessRules)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionBusinessRules = subscriptionBusinessRules;
    }

    public async Task<Subscription?> GetAsync(
        Expression<Func<Subscription, bool>> predicate,
        Func<IQueryable<Subscription>, IIncludableQueryable<Subscription, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Subscription? subscription = await _subscriptionRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return subscription;
    }

    public async Task<IPaginate<Subscription>?> GetListAsync(
        Expression<Func<Subscription, bool>>? predicate = null,
        Func<IQueryable<Subscription>, IOrderedQueryable<Subscription>>? orderBy = null,
        Func<IQueryable<Subscription>, IIncludableQueryable<Subscription, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Subscription> subscriptionList = await _subscriptionRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return subscriptionList;
    }

    public async Task<Subscription> AddAsync(Subscription subscription)
    {
        Subscription addedSubscription = await _subscriptionRepository.AddAsync(subscription);

        return addedSubscription;
    }

    public async Task<Subscription> UpdateAsync(Subscription subscription)
    {
        Subscription updatedSubscription = await _subscriptionRepository.UpdateAsync(subscription);

        return updatedSubscription;
    }

    public async Task<Subscription> DeleteAsync(Subscription subscription, bool permanent = false)
    {
        Subscription deletedSubscription = await _subscriptionRepository.DeleteAsync(subscription);

        return deletedSubscription;
    }
}
