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
using Application.Features.PuantajRecords.Rules;

namespace Application.Features.PuantajRecords.Queries.GetList;

public class GetListPuantajRecordQuery : IRequest<GetListResponse<GetListPuantajRecordListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? SiteId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListPuantajRecordQueryHandler : IRequestHandler<GetListPuantajRecordQuery, GetListResponse<GetListPuantajRecordListItemDto>>
    {
        private readonly IPuantajRecordRepository _puantajRecordRepository;
        private readonly IMapper _mapper;

        private readonly PuantajRecordBusinessRules _rules;

        public GetListPuantajRecordQueryHandler(
            IPuantajRecordRepository puantajRecordRepository, IMapper mapper, PuantajRecordBusinessRules rules)
        {
            _puantajRecordRepository = puantajRecordRepository;
            _mapper = mapper;
            _rules = rules;
        }

        public async Task<GetListResponse<GetListPuantajRecordListItemDto>> Handle(GetListPuantajRecordQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await _rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            IPaginate<PuantajRecord> puantajRecords = await _puantajRecordRepository.GetListAsync(
                predicate: x => (!tenantId.HasValue || x.TenantId == tenantId.Value)
                    && (!request.ProjectId.HasValue || x.ProjectId == request.ProjectId.Value)
                    && (!request.SiteId.HasValue || x.SiteId == request.SiteId.Value),
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListPuantajRecordListItemDto> response = _mapper.Map<GetListResponse<GetListPuantajRecordListItemDto>>(puantajRecords);
            return response;
        }
    }
}