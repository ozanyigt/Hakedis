using Application.Services.HakedisPeriods;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisDeductionLines.Commands.Delete;

public class DeleteHakedisDeductionLineCommand
    : IRequest<DeletedHakedisDeductionLineResponse>,
        ISecuredRequest,
        ILoggableRequest,
        ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write];

    public class DeleteHakedisDeductionLineCommandHandler
        : IRequestHandler<DeleteHakedisDeductionLineCommand, DeletedHakedisDeductionLineResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisDeductionLineRepository _repository;
        private readonly IHakedisPeriodTotalsService _totalsService;

        public DeleteHakedisDeductionLineCommandHandler(
            IMapper mapper,
            IHakedisDeductionLineRepository repository,
            IHakedisPeriodTotalsService totalsService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _totalsService = totalsService;
        }

        public async Task<DeletedHakedisDeductionLineResponse> Handle(
            DeleteHakedisDeductionLineCommand request,
            CancellationToken cancellationToken
        )
        {
            HakedisDeductionLine? line = await _repository.GetAsync(
                predicate: l => l.Id == request.Id,
                cancellationToken: cancellationToken
            );

            if (line is null)
                throw new InvalidOperationException("Kesinti satırı bulunamadı.");

            Guid periodId = line.HakedisPeriodId;
            await _repository.DeleteAsync(line);
            await _totalsService.SyncDeductionTotalsAsync(periodId, cancellationToken);

            return _mapper.Map<DeletedHakedisDeductionLineResponse>(line);
        }
    }
}

public class DeletedHakedisDeductionLineResponse
{
    public Guid Id { get; set; }
}
