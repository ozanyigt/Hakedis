using Application.Features.PuantajRecords.Constants;
using Application.Features.PuantajRecords.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.PuantajRecords.Constants.PuantajRecordsOperationClaims;

namespace Application.Features.PuantajRecords.Commands.Update;

public class UpdatePuantajRecordCommand : IRequest<UpdatedPuantajRecordResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? WorkerId { get; set; }
    public required DateTime WorkDate { get; set; }
    public required string WorkType { get; set; }
    public required decimal DayCount { get; set; }
    public required decimal OvertimeHours { get; set; }
    public required PuantajStatus Status { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, PuantajRecordsOperationClaims.Update];

    public class UpdatePuantajRecordCommandHandler : IRequestHandler<UpdatePuantajRecordCommand, UpdatedPuantajRecordResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly PuantajRecordBusinessRules _puantajRecordBusinessRules;

        public UpdatePuantajRecordCommandHandler(IMapper mapper, IPuantajRecordRepository puantajRecordRepository,
                                         PuantajRecordBusinessRules puantajRecordBusinessRules)
        {
            _mapper = mapper;
            _puantajRecordRepository = puantajRecordRepository;
            _puantajRecordBusinessRules = puantajRecordBusinessRules;
        }

        public async Task<UpdatedPuantajRecordResponse> Handle(UpdatePuantajRecordCommand request, CancellationToken cancellationToken)
        {
            PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(predicate: pr => pr.Id == request.Id, cancellationToken: cancellationToken);
            await _puantajRecordBusinessRules.PuantajRecordShouldExistWhenSelected(puantajRecord);
            puantajRecord = _mapper.Map(request, puantajRecord);

            await _puantajRecordRepository.UpdateAsync(puantajRecord!);

            UpdatedPuantajRecordResponse response = _mapper.Map<UpdatedPuantajRecordResponse>(puantajRecord);
            return response;
        }
    }
}