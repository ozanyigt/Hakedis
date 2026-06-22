using Application.Features.PuantajRecords.Constants;
using Application.Features.PuantajRecords.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.PuantajRecords.Constants.PuantajRecordsOperationClaims;

namespace Application.Features.PuantajRecords.Queries.GetById;

public class GetByIdPuantajRecordQuery : IRequest<GetByIdPuantajRecordResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdPuantajRecordQueryHandler : IRequestHandler<GetByIdPuantajRecordQuery, GetByIdPuantajRecordResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly PuantajRecordBusinessRules _puantajRecordBusinessRules;

        public GetByIdPuantajRecordQueryHandler(IMapper mapper, IPuantajRecordRepository puantajRecordRepository, PuantajRecordBusinessRules puantajRecordBusinessRules)
        {
            _mapper = mapper;
            _puantajRecordRepository = puantajRecordRepository;
            _puantajRecordBusinessRules = puantajRecordBusinessRules;
        }

        public async Task<GetByIdPuantajRecordResponse> Handle(GetByIdPuantajRecordQuery request, CancellationToken cancellationToken)
        {
            PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(predicate: pr => pr.Id == request.Id, cancellationToken: cancellationToken);
            await _puantajRecordBusinessRules.PuantajRecordShouldExistWhenSelected(puantajRecord);

            GetByIdPuantajRecordResponse response = _mapper.Map<GetByIdPuantajRecordResponse>(puantajRecord);
            return response;
        }
    }
}