using Application.Features.HakedisDeductionLines.Constants;
using Application.Services.HakedisPeriods;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisDeductionLines.Commands.Create;

public class CreateHakedisDeductionLineCommand
    : IRequest<CreatedHakedisDeductionLineResponse>,
        ISecuredRequest,
        ILoggableRequest,
        ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid HakedisPeriodId { get; set; }
    public required DeductionCategory Category { get; set; }
    public required string Description { get; set; }
    public required decimal Amount { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write];

    public class CreateHakedisDeductionLineCommandHandler
        : IRequestHandler<CreateHakedisDeductionLineCommand, CreatedHakedisDeductionLineResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisDeductionLineRepository _repository;
        private readonly IHakedisPeriodTotalsService _totalsService;

        public CreateHakedisDeductionLineCommandHandler(
            IMapper mapper,
            IHakedisDeductionLineRepository repository,
            IHakedisPeriodTotalsService totalsService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _totalsService = totalsService;
        }

        public async Task<CreatedHakedisDeductionLineResponse> Handle(
            CreateHakedisDeductionLineCommand request,
            CancellationToken cancellationToken
        )
        {
            HakedisDeductionLine line = _mapper.Map<HakedisDeductionLine>(request);
            await _repository.AddAsync(line);
            await _totalsService.SyncDeductionTotalsAsync(request.HakedisPeriodId, cancellationToken);

            return _mapper.Map<CreatedHakedisDeductionLineResponse>(line);
        }
    }
}

public class CreatedHakedisDeductionLineResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public DeductionCategory Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
