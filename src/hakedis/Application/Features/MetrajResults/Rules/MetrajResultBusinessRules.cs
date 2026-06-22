using Application.Features.MetrajResults.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.MetrajResults.Rules;

public class MetrajResultBusinessRules : BaseBusinessRules
{
    private readonly IMetrajResultRepository _metrajResultRepository;
    private readonly ILocalizationService _localizationService;

    public MetrajResultBusinessRules(IMetrajResultRepository metrajResultRepository, ILocalizationService localizationService)
    {
        _metrajResultRepository = metrajResultRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, MetrajResultsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task MetrajResultShouldExistWhenSelected(MetrajResult? metrajResult)
    {
        if (metrajResult == null)
            await throwBusinessException(MetrajResultsBusinessMessages.MetrajResultNotExists);
    }

    public async Task MetrajResultIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        MetrajResult? metrajResult = await _metrajResultRepository.GetAsync(
            predicate: mr => mr.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await MetrajResultShouldExistWhenSelected(metrajResult);
    }
}