using Application.Features.Drawings.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.MetrajCalculation;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Queries.GetLayers;

public class GetDrawingLayersQuery : IRequest<DrawingLayersDiscoveryResultDto>, ISecuredRequest
{
    public Guid DrawingId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetDrawingLayersQueryHandler : IRequestHandler<GetDrawingLayersQuery, DrawingLayersDiscoveryResultDto>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMetrajCalculationService _metrajCalculationService;
        private readonly DrawingBusinessRules _drawingBusinessRules;

        public GetDrawingLayersQueryHandler(
            IDrawingRepository drawingRepository,
            IMetrajCalculationService metrajCalculationService,
            DrawingBusinessRules drawingBusinessRules
        )
        {
            _drawingRepository = drawingRepository;
            _metrajCalculationService = metrajCalculationService;
            _drawingBusinessRules = drawingBusinessRules;
        }

        public async Task<DrawingLayersDiscoveryResultDto> Handle(
            GetDrawingLayersQuery request,
            CancellationToken cancellationToken
        )
        {
            Drawing? drawing = await _drawingRepository.GetAsync(
                predicate: item => item.Id == request.DrawingId,
                cancellationToken: cancellationToken
            );
            await _drawingBusinessRules.DrawingShouldExistWhenSelected(drawing);

            return await _metrajCalculationService.DiscoverLayersAsync(
                drawing!.FilePath,
                drawing.FileExtension,
                cancellationToken
            );
        }
    }
}
