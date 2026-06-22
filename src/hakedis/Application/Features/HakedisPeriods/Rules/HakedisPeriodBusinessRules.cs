using Application.Features.HakedisPeriods.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.HakedisPeriods.Rules;

public class HakedisPeriodBusinessRules : BaseBusinessRules
{
    private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
    private readonly ILocalizationService _localizationService;

    public HakedisPeriodBusinessRules(IHakedisPeriodRepository hakedisPeriodRepository, ILocalizationService localizationService)
    {
        _hakedisPeriodRepository = hakedisPeriodRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, HakedisPeriodsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task HakedisPeriodShouldExistWhenSelected(HakedisPeriod? hakedisPeriod)
    {
        if (hakedisPeriod == null)
            await throwBusinessException(HakedisPeriodsBusinessMessages.HakedisPeriodNotExists);
    }

    public async Task HakedisPeriodIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        HakedisPeriod? hakedisPeriod = await _hakedisPeriodRepository.GetAsync(
            predicate: hp => hp.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await HakedisPeriodShouldExistWhenSelected(hakedisPeriod);
    }
}