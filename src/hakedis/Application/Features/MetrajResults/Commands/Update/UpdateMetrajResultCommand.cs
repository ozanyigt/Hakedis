using Application.Features.MetrajResults.Constants;
using Application.Features.MetrajResults.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Commands.Update;

public class UpdateMetrajResultCommand : IRequest<UpdatedMetrajResultResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public required Guid DrawingId { get; set; }
    public required MetrajKalemType KalemType { get; set; }
    public required string Unit { get; set; }
    public required decimal Quantity { get; set; }
    public string? FloorName { get; set; }
    public string? SpaceName { get; set; }
    public required DateTime CalculatedAt { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, MetrajResultsOperationClaims.Update];

    public class UpdateMetrajResultCommandHandler : IRequestHandler<UpdateMetrajResultCommand, UpdatedMetrajResultResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly MetrajResultBusinessRules _metrajResultBusinessRules;

        public UpdateMetrajResultCommandHandler(IMapper mapper, IMetrajResultRepository metrajResultRepository,
                                         MetrajResultBusinessRules metrajResultBusinessRules)
        {
            _mapper = mapper;
            _metrajResultRepository = metrajResultRepository;
            _metrajResultBusinessRules = metrajResultBusinessRules;
        }

        public async Task<UpdatedMetrajResultResponse> Handle(UpdateMetrajResultCommand request, CancellationToken cancellationToken)
        {
            MetrajResult? metrajResult = await _metrajResultRepository.GetAsync(predicate: mr => mr.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajResultBusinessRules.MetrajResultShouldExistWhenSelected(metrajResult);
            metrajResult = _mapper.Map(request, metrajResult);

            await _metrajResultRepository.UpdateAsync(metrajResult!);

            UpdatedMetrajResultResponse response = _mapper.Map<UpdatedMetrajResultResponse>(metrajResult);
            return response;
        }
    }
}