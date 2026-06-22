using Application.Features.ContractItems.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.ContractItems.Rules;

public class ContractItemBusinessRules : BaseBusinessRules
{
    private readonly IContractItemRepository _contractItemRepository;
    private readonly ILocalizationService _localizationService;

    public ContractItemBusinessRules(IContractItemRepository contractItemRepository, ILocalizationService localizationService)
    {
        _contractItemRepository = contractItemRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ContractItemsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ContractItemShouldExistWhenSelected(ContractItem? contractItem)
    {
        if (contractItem == null)
            await throwBusinessException(ContractItemsBusinessMessages.ContractItemNotExists);
    }

    public async Task ContractItemIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        ContractItem? contractItem = await _contractItemRepository.GetAsync(
            predicate: ci => ci.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await ContractItemShouldExistWhenSelected(contractItem);
    }
}