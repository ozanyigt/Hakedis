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

namespace Application.Features.MetrajResults.Commands.Create;

public class CreateMetrajResultCommand : IRequest<CreatedMetrajResultResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public required Guid DrawingId { get; set; }
    public required MetrajKalemType KalemType { get; set; }
    public required MeasurementUnit Unit { get; set; }
    public required decimal Quantity { get; set; }
    public string? FloorName { get; set; }
    public string? SpaceName { get; set; }
    public required DateTime CalculatedAt { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, MetrajResultsOperationClaims.Create];

    public class CreateMetrajResultCommandHandler : IRequestHandler<CreateMetrajResultCommand, CreatedMetrajResultResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly MetrajResultBusinessRules _metrajResultBusinessRules;

        public CreateMetrajResultCommandHandler(IMapper mapper, IMetrajResultRepository metrajResultRepository,
                                         MetrajResultBusinessRules metrajResultBusinessRules)
        {
            _mapper = mapper;
            _metrajResultRepository = metrajResultRepository;
            _metrajResultBusinessRules = metrajResultBusinessRules;
        }

        public async Task<CreatedMetrajResultResponse> Handle(CreateMetrajResultCommand request, CancellationToken cancellationToken)
        {
            MetrajResult metrajResult = _mapper.Map<MetrajResult>(request);

            await _metrajResultRepository.AddAsync(metrajResult);

            CreatedMetrajResultResponse response = _mapper.Map<CreatedMetrajResultResponse>(metrajResult);
            return response;
        }
    }
}