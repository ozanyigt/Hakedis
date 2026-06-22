using Application.Features.PuantajRecords.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.PuantajRecords.Constants.PuantajRecordsOperationClaims;

namespace Application.Features.PuantajRecords.Queries.GetList;

public class GetListPuantajRecordQuery : IRequest<GetListResponse<GetListPuantajRecordListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListPuantajRecordQueryHandler : IRequestHandler<GetListPuantajRecordQuery, GetListResponse<GetListPuantajRecordListItemDto>>
    {
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly IMapper _mapper;

        public GetListPuantajRecordQueryHandler(IPuantajRecordRepository puantajRecordRepository, IMapper mapper)
        {
            _puantajRecordRepository = puantajRecordRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListPuantajRecordListItemDto>> Handle(GetListPuantajRecordQuery request, CancellationToken cancellationToken)
        {
            IPaginate<PuantajRecord> puantajRecords = await _puantajRecordRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListPuantajRecordListItemDto> response = _mapper.Map<GetListResponse<GetListPuantajRecordListItemDto>>(puantajRecords);
            return response;
        }
    }
}