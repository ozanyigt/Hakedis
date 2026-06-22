using Application.Features.Drawings.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Commands.Delete;

public class DeleteDrawingCommand : IRequest<DeletedDrawingResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, DrawingsOperationClaims.Delete];

    public class DeleteDrawingCommandHandler : IRequestHandler<DeleteDrawingCommand, DeletedDrawingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly DrawingBusinessRules _drawingBusinessRules;

        public DeleteDrawingCommandHandler(
            IMapper mapper,
            IDrawingRepository drawingRepository,
            IMetrajResultRepository metrajResultRepository,
            DrawingBusinessRules drawingBusinessRules
        )
        {
            _mapper = mapper;
            _drawingRepository = drawingRepository;
            _metrajResultRepository = metrajResultRepository;
            _drawingBusinessRules = drawingBusinessRules;
        }

        public async Task<DeletedDrawingResponse> Handle(DeleteDrawingCommand request, CancellationToken cancellationToken)
        {
            Drawing? drawing = await _drawingRepository.GetAsync(
                predicate: d => d.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _drawingBusinessRules.DrawingShouldExistWhenSelected(drawing);

            IPaginate<MetrajResult> metrajResults = await _metrajResultRepository.GetListAsync(
                predicate: r => r.DrawingId == request.Id,
                index: 0,
                size: 500,
                cancellationToken: cancellationToken
            );

            foreach (MetrajResult result in metrajResults.Items)
                await _metrajResultRepository.DeleteAsync(result);

            if (!string.IsNullOrWhiteSpace(drawing!.FilePath) && File.Exists(drawing.FilePath))
            {
                try
                {
                    File.Delete(drawing.FilePath);
                }
                catch
                {
                    // Disk dosyası silinemese de kayıt silinsin.
                }
            }

            await _drawingRepository.DeleteAsync(drawing);

            DeletedDrawingResponse response = _mapper.Map<DeletedDrawingResponse>(drawing);
            return response;
        }
    }
}