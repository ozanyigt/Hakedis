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

namespace Application.Features.HakedisDeductionLines.Commands.Update;

public class UpdateHakedisDeductionLineCommand
    : IRequest<UpdatedHakedisDeductionLineResponse>,
        ISecuredRequest,
        ILoggableRequest,
        ITransactionalRequest
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid HakedisPeriodId { get; set; }
    public required DeductionCategory Category { get; set; }
    public required string Description { get; set; }
    public required decimal Amount { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write];

    public class UpdateHakedisDeductionLineCommandHandler
        : IRequestHandler<UpdateHakedisDeductionLineCommand, UpdatedHakedisDeductionLineResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisDeductionLineRepository _repository;
        private readonly IHakedisPeriodTotalsService _totalsService;

        public UpdateHakedisDeductionLineCommandHandler(
            IMapper mapper,
            IHakedisDeductionLineRepository repository,
            IHakedisPeriodTotalsService totalsService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _totalsService = totalsService;
        }

        public async Task<UpdatedHakedisDeductionLineResponse> Handle(
            UpdateHakedisDeductionLineCommand request,
            CancellationToken cancellationToken
        )
        {
            HakedisDeductionLine? line = await _repository.GetAsync(
                predicate: l => l.Id == request.Id,
                cancellationToken: cancellationToken
            );

            if (line is null)
                throw new InvalidOperationException("Kesinti satırı bulunamadı.");

            _mapper.Map(request, line);
            await _repository.UpdateAsync(line);
            await _totalsService.SyncDeductionTotalsAsync(request.HakedisPeriodId, cancellationToken);

            return _mapper.Map<UpdatedHakedisDeductionLineResponse>(line);
        }
    }
}

public class UpdatedHakedisDeductionLineResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public DeductionCategory Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
