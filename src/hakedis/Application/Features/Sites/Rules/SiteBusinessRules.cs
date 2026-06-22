using Application.Features.Sites.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Sites.Rules;

public class SiteBusinessRules : BaseBusinessRules
{
    private readonly ISiteRepository _siteRepository;
    private readonly ILocalizationService _localizationService;

    public SiteBusinessRules(ISiteRepository siteRepository, ILocalizationService localizationService)
    {
        _siteRepository = siteRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, SitesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task SiteShouldExistWhenSelected(Site? site)
    {
        if (site == null)
            await throwBusinessException(SitesBusinessMessages.SiteNotExists);
    }

    public async Task SiteIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Site? site = await _siteRepository.GetAsync(
            predicate: s => s.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await SiteShouldExistWhenSelected(site);
    }
}