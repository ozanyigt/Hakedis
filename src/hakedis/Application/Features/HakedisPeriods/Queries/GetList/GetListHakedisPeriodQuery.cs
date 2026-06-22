using Application.Features.HakedisPeriods.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisPeriods.Queries.GetList;

public class GetListHakedisPeriodQuery : IRequest<GetListResponse<GetListHakedisPeriodListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListHakedisPeriodQueryHandler : IRequestHandler<GetListHakedisPeriodQuery, GetListResponse<GetListHakedisPeriodListItemDto>>
    {
        private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
        private readonly IMapper _mapper;

        public GetListHakedisPeriodQueryHandler(IHakedisPeriodRepository hakedisPeriodRepository, IMapper mapper)
        {
            _hakedisPeriodRepository = hakedisPeriodRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListHakedisPeriodListItemDto>> Handle(GetListHakedisPeriodQuery request, CancellationToken cancellationToken)
        {
            IPaginate<HakedisPeriod> hakedisPeriods = await _hakedisPeriodRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListHakedisPeriodListItemDto> response = _mapper.Map<GetListResponse<GetListHakedisPeriodListItemDto>>(hakedisPeriods);
            return response;
        }
    }
}