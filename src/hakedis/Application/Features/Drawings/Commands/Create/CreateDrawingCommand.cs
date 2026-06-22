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

namespace Application.Features.Drawings.Commands.Create;

public class CreateDrawingCommand : IRequest<CreatedDrawingResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
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

    public string[] Roles => [Admin, Write, DrawingsOperationClaims.Create];

    public class CreateDrawingCommandHandler : IRequestHandler<CreateDrawingCommand, CreatedDrawingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDrawingRepository _drawingRepository;
        private readonly DrawingBusinessRules _drawingBusinessRules;

        public CreateDrawingCommandHandler(IMapper mapper, IDrawingRepository drawingRepository,
                                         DrawingBusinessRules drawingBusinessRules)
        {
            _mapper = mapper;
            _drawingRepository = drawingRepository;
            _drawingBusinessRules = drawingBusinessRules;
        }

        public async Task<CreatedDrawingResponse> Handle(CreateDrawingCommand request, CancellationToken cancellationToken)
        {
            Drawing drawing = _mapper.Map<Drawing>(request);

            await _drawingRepository.AddAsync(drawing);

            CreatedDrawingResponse response = _mapper.Map<CreatedDrawingResponse>(drawing);
            return response;
        }
    }
}