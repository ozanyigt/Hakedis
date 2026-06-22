using Application.Features.Drawings.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Queries.GetById;

public class GetByIdDrawingQuery : IRequest<GetByIdDrawingResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdDrawingQueryHandler : IRequestHandler<GetByIdDrawingQuery, GetByIdDrawingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDrawingRepository _drawingRepository;
        private readonly DrawingBusinessRules _drawingBusinessRules;

        public GetByIdDrawingQueryHandler(IMapper mapper, IDrawingRepository drawingRepository, DrawingBusinessRules drawingBusinessRules)
        {
            _mapper = mapper;
            _drawingRepository = drawingRepository;
            _drawingBusinessRules = drawingBusinessRules;
        }

        public async Task<GetByIdDrawingResponse> Handle(GetByIdDrawingQuery request, CancellationToken cancellationToken)
        {
            Drawing? drawing = await _drawingRepository.GetAsync(predicate: d => d.Id == request.Id, cancellationToken: cancellationToken);
            await _drawingBusinessRules.DrawingShouldExistWhenSelected(drawing);

            GetByIdDrawingResponse response = _mapper.Map<GetByIdDrawingResponse>(drawing);
            return response;
        }
    }
}