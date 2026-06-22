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

namespace Application.Features.ProgressEntries.Commands.Create;

public class CreateProgressEntryCommand : IRequest<CreatedProgressEntryResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid HakedisPeriodId { get; set; }
    public required Guid ContractItemId { get; set; }
    public required decimal QuantityThisPeriod { get; set; }
    public required decimal CumulativeQuantity { get; set; }
    public required decimal AmountThisPeriod { get; set; }
    public Guid? MetrajResultId { get; set; }
    public required bool IsManualEntry { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, ProgressEntriesOperationClaims.Create];

    public class CreateProgressEntryCommandHandler : IRequestHandler<CreateProgressEntryCommand, CreatedProgressEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ProgressEntryBusinessRules _progressEntryBusinessRules;

        public CreateProgressEntryCommandHandler(IMapper mapper, IProgressEntryRepository progressEntryRepository,
                                         ProgressEntryBusinessRules progressEntryBusinessRules)
        {
            _mapper = mapper;
            _progressEntryRepository = progressEntryRepository;
            _progressEntryBusinessRules = progressEntryBusinessRules;
        }

        public async Task<CreatedProgressEntryResponse> Handle(CreateProgressEntryCommand request, CancellationToken cancellationToken)
        {
            ProgressEntry progressEntry = _mapper.Map<ProgressEntry>(request);

            await _progressEntryRepository.AddAsync(progressEntry);

            CreatedProgressEntryResponse response = _mapper.Map<CreatedProgressEntryResponse>(progressEntry);
            return response;
        }
    }
}