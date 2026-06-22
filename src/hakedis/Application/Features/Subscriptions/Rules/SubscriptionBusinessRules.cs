using Application.Features.Subscriptions.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Subscriptions.Rules;

public class SubscriptionBusinessRules : BaseBusinessRules
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILocalizationService _localizationService;

    public SubscriptionBusinessRules(ISubscriptionRepository subscriptionRepository, ILocalizationService localizationService)
    {
        _subscriptionRepository = subscriptionRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, SubscriptionsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task SubscriptionShouldExistWhenSelected(Subscription? subscription)
    {
        if (subscription == null)
            await throwBusinessException(SubscriptionsBusinessMessages.SubscriptionNotExists);
    }

    public async Task SubscriptionIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Subscription? subscription = await _subscriptionRepository.GetAsync(
            predicate: s => s.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await SubscriptionShouldExistWhenSelected(subscription);
    }
}