using Application.Features.PuantajRecords.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Pipelines.Authorization;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.PuantajRecords.Constants.PuantajRecordsOperationClaims;

namespace Application.Features.PuantajRecords.Queries.GetListByDynamic;

public class GetListByDynamicPuantajRecordQuery : IRequest<GetListResponse<GetListByDynamicPuantajRecordListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicPuantajRecordQueryHandler : IRequestHandler<GetListByDynamicPuantajRecordQuery, GetListResponse<GetListByDynamicPuantajRecordListItemDto>>
    {
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicPuantajRecordQueryHandler(IPuantajRecordRepository puantajRecordRepository, IMapper mapper)
        {
            _puantajRecordRepository = puantajRecordRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicPuantajRecordListItemDto>> Handle(GetListByDynamicPuantajRecordQuery request, CancellationToken cancellationToken)
        {
            IPaginate<PuantajRecord> puantajRecords = await _puantajRecordRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicPuantajRecordListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicPuantajRecordListItemDto>>(puantajRecords);
            return response;
        }
    }
}
