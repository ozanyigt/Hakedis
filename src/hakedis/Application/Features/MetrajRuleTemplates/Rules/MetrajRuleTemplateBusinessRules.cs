using Application.Features.MetrajRuleTemplates.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.MetrajRuleTemplates.Rules;

public class MetrajRuleTemplateBusinessRules : BaseBusinessRules
{
    private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
    private readonly ILocalizationService _localizationService;

    public MetrajRuleTemplateBusinessRules(IMetrajRuleTemplateRepository metrajRuleTemplateRepository, ILocalizationService localizationService)
    {
        _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, MetrajRuleTemplatesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task MetrajRuleTemplateShouldExistWhenSelected(MetrajRuleTemplate? metrajRuleTemplate)
    {
        if (metrajRuleTemplate == null)
            await throwBusinessException(MetrajRuleTemplatesBusinessMessages.MetrajRuleTemplateNotExists);
    }

    public async Task MetrajRuleTemplateIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        MetrajRuleTemplate? metrajRuleTemplate = await _metrajRuleTemplateRepository.GetAsync(
            predicate: mrt => mrt.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await MetrajRuleTemplateShouldExistWhenSelected(metrajRuleTemplate);
    }
}