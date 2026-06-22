using Application.Features.HakedisPeriods.Constants;
using Application.Features.HakedisPeriods.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisPeriods.Commands.Update;

public class UpdateHakedisPeriodCommand : IRequest<UpdatedHakedisPeriodResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public required int PeriodNumber { get; set; }
    public required string Name { get; set; }
    public required DateTime PeriodStart { get; set; }
    public required DateTime PeriodEnd { get; set; }
    public required HakedisStatus Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public required decimal DeductionAmount { get; set; }
    public required decimal NetAmount { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, HakedisPeriodsOperationClaims.Update];

    public class UpdateHakedisPeriodCommandHandler : IRequestHandler<UpdateHakedisPeriodCommand, UpdatedHakedisPeriodResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
        private readonly HakedisPeriodBusinessRules _hakedisPeriodBusinessRules;

        public UpdateHakedisPeriodCommandHandler(IMapper mapper, IHakedisPeriodRepository hakedisPeriodRepository,
                                         HakedisPeriodBusinessRules hakedisPeriodBusinessRules)
        {
            _mapper = mapper;
            _hakedisPeriodRepository = hakedisPeriodRepository;
            _hakedisPeriodBusinessRules = hakedisPeriodBusinessRules;
        }

        public async Task<UpdatedHakedisPeriodResponse> Handle(UpdateHakedisPeriodCommand request, CancellationToken cancellationToken)
        {
            HakedisPeriod? hakedisPeriod = await _hakedisPeriodRepository.GetAsync(predicate: hp => hp.Id == request.Id, cancellationToken: cancellationToken);
            await _hakedisPeriodBusinessRules.HakedisPeriodShouldExistWhenSelected(hakedisPeriod);
            hakedisPeriod = _mapper.Map(request, hakedisPeriod);

            await _hakedisPeriodRepository.UpdateAsync(hakedisPeriod!);

            UpdatedHakedisPeriodResponse response = _mapper.Map<UpdatedHakedisPeriodResponse>(hakedisPeriod);
            return response;
        }
    }
}