using Application.Features.Drawings.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Commands.Update;

public class UpdateDrawingCommand : IRequest<UpdatedDrawingResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string FileExtension { get; set; }
    public required long FileSizeBytes { get; set; }
    public required DrawingStatus Status { get; set; }
    public string? ParseErrorMessage { get; set; }
    public DateTime? ParsedAt { get; set; }

    public string[] Roles => [Admin, Write, DrawingsOperationClaims.Update];

    public class UpdateDrawingCommandHandler : IRequestHandler<UpdateDrawingCommand, UpdatedDrawingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDrawingRepository _drawingRepository;
        private readonly DrawingBusinessRules _drawingBusinessRules;

        public UpdateDrawingCommandHandler(IMapper mapper, IDrawingRepository drawingRepository,
                                         DrawingBusinessRules drawingBusinessRules)
        {
            _mapper = mapper;
            _drawingRepository = drawingRepository;
            _drawingBusinessRules = drawingBusinessRules;
        }

        public async Task<UpdatedDrawingResponse> Handle(UpdateDrawingCommand request, CancellationToken cancellationToken)
        {
            Drawing? drawing = await _drawingRepository.GetAsync(predicate: d => d.Id == request.Id, cancellationToken: cancellationToken);
            await _drawingBusinessRules.DrawingShouldExistWhenSelected(drawing);
            drawing = _mapper.Map(request, drawing);

            await _drawingRepository.UpdateAsync(drawing!);

            UpdatedDrawingResponse response = _mapper.Map<UpdatedDrawingResponse>(drawing);
            return response;
        }
    }
}