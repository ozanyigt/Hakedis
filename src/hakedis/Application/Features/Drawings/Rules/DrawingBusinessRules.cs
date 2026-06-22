using Application.Features.Drawings.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Drawings.Rules;

public class DrawingBusinessRules : BaseBusinessRules
{
    private readonly IDrawingRepository _drawingRepository;
    private readonly ILocalizationService _localizationService;

    public DrawingBusinessRules(IDrawingRepository drawingRepository, ILocalizationService localizationService)
    {
        _drawingRepository = drawingRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, DrawingsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task DrawingShouldExistWhenSelected(Drawing? drawing)
    {
        if (drawing == null)
            await throwBusinessException(DrawingsBusinessMessages.DrawingNotExists);
    }

    public async Task DrawingIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Drawing? drawing = await _drawingRepository.GetAsync(
            predicate: d => d.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await DrawingShouldExistWhenSelected(drawing);
    }
}