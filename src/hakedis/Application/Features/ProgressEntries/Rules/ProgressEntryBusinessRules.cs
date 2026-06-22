using Application.Features.ProgressEntries.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.ProgressEntries.Rules;

public class ProgressEntryBusinessRules : BaseBusinessRules
{
    private readonly IProgressEntryRepository _progressEntryRepository;
    private readonly ILocalizationService _localizationService;

    public ProgressEntryBusinessRules(IProgressEntryRepository progressEntryRepository, ILocalizationService localizationService)
    {
        _progressEntryRepository = progressEntryRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ProgressEntriesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ProgressEntryShouldExistWhenSelected(ProgressEntry? progressEntry)
    {
        if (progressEntry == null)
            await throwBusinessException(ProgressEntriesBusinessMessages.ProgressEntryNotExists);
    }

    public async Task ProgressEntryIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        ProgressEntry? progressEntry = await _progressEntryRepository.GetAsync(
            predicate: pe => pe.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await ProgressEntryShouldExistWhenSelected(progressEntry);
    }
}