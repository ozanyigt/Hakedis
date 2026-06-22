using Application.Features.SubscriptionPlans.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.SubscriptionPlans.Rules;

public class SubscriptionPlanBusinessRules : BaseBusinessRules
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly ILocalizationService _localizationService;

    public SubscriptionPlanBusinessRules(ISubscriptionPlanRepository subscriptionPlanRepository, ILocalizationService localizationService)
    {
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, SubscriptionPlansBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task SubscriptionPlanShouldExistWhenSelected(SubscriptionPlan? subscriptionPlan)
    {
        if (subscriptionPlan == null)
            await throwBusinessException(SubscriptionPlansBusinessMessages.SubscriptionPlanNotExists);
    }

    public async Task SubscriptionPlanIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetAsync(
            predicate: sp => sp.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await SubscriptionPlanShouldExistWhenSelected(subscriptionPlan);
    }
}