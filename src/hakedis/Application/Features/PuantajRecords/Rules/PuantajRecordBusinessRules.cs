using Application.Features.PuantajRecords.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.PuantajRecords.Rules;

public class PuantajRecordBusinessRules : BaseBusinessRules
{
    private readonly IPuantajRecordRepository _puantajRecordRepository;
    private readonly ILocalizationService _localizationService;

    public PuantajRecordBusinessRules(IPuantajRecordRepository puantajRecordRepository, ILocalizationService localizationService)
    {
        _puantajRecordRepository = puantajRecordRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, PuantajRecordsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task PuantajRecordShouldExistWhenSelected(PuantajRecord? puantajRecord)
    {
        if (puantajRecord == null)
            await throwBusinessException(PuantajRecordsBusinessMessages.PuantajRecordNotExists);
    }

    public async Task PuantajRecordIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(
            predicate: pr => pr.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await PuantajRecordShouldExistWhenSelected(puantajRecord);
    }
}