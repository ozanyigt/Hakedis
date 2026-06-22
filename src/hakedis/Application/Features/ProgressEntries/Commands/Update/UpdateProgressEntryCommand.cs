using Application.Features.ProgressEntries.Constants;
using Application.Features.ProgressEntries.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.ProgressEntries.Constants.ProgressEntriesOperationClaims;

namespace Application.Features.ProgressEntries.Commands.Update;

public class UpdateProgressEntryCommand : IRequest<UpdatedProgressEntryResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid HakedisPeriodId { get; set; }
    public required Guid ContractItemId { get; set; }
    public required decimal QuantityThisPeriod { get; set; }
    public required decimal CumulativeQuantity { get; set; }
    public required decimal AmountThisPeriod { get; set; }
    public Guid? MetrajResultId { get; set; }
    public required bool IsManualEntry { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, ProgressEntriesOperationClaims.Update];

    public class UpdateProgressEntryCommandHandler : IRequestHandler<UpdateProgressEntryCommand, UpdatedProgressEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ProgressEntryBusinessRules _progressEntryBusinessRules;

        public UpdateProgressEntryCommandHandler(IMapper mapper, IProgressEntryRepository progressEntryRepository,
                                         ProgressEntryBusinessRules progressEntryBusinessRules)
        {
            _mapper = mapper;
            _progressEntryRepository = progressEntryRepository;
            _progressEntryBusinessRules = progressEntryBusinessRules;
        }

        public async Task<UpdatedProgressEntryResponse> Handle(UpdateProgressEntryCommand request, CancellationToken cancellationToken)
        {
            ProgressEntry? progressEntry = await _progressEntryRepository.GetAsync(predicate: pe => pe.Id == request.Id, cancellationToken: cancellationToken);
            await _progressEntryBusinessRules.ProgressEntryShouldExistWhenSelected(progressEntry);
            progressEntry = _mapper.Map(request, progressEntry);

            await _progressEntryRepository.UpdateAsync(progressEntry!);

            UpdatedProgressEntryResponse response = _mapper.Map<UpdatedProgressEntryResponse>(progressEntry);
            return response;
        }
    }
}