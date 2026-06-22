using Application.Features.PuantajRecords.Constants;
using Application.Features.PuantajRecords.Constants;
using Application.Features.PuantajRecords.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.PuantajRecords.Constants.PuantajRecordsOperationClaims;

namespace Application.Features.PuantajRecords.Commands.Delete;

public class DeletePuantajRecordCommand : IRequest<DeletedPuantajRecordResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, PuantajRecordsOperationClaims.Delete];

    public class DeletePuantajRecordCommandHandler : IRequestHandler<DeletePuantajRecordCommand, DeletedPuantajRecordResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly PuantajRecordBusinessRules _puantajRecordBusinessRules;

        public DeletePuantajRecordCommandHandler(IMapper mapper, IPuantajRecordRepository puantajRecordRepository,
                                         PuantajRecordBusinessRules puantajRecordBusinessRules)
        {
            _mapper = mapper;
            _puantajRecordRepository = puantajRecordRepository;
            _puantajRecordBusinessRules = puantajRecordBusinessRules;
        }

        public async Task<DeletedPuantajRecordResponse> Handle(DeletePuantajRecordCommand request, CancellationToken cancellationToken)
        {
            PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(predicate: pr => pr.Id == request.Id, cancellationToken: cancellationToken);
            await _puantajRecordBusinessRules.PuantajRecordShouldExistWhenSelected(puantajRecord);

            await _puantajRecordRepository.DeleteAsync(puantajRecord!);

            DeletedPuantajRecordResponse response = _mapper.Map<DeletedPuantajRecordResponse>(puantajRecord);
            return response;
        }
    }
}