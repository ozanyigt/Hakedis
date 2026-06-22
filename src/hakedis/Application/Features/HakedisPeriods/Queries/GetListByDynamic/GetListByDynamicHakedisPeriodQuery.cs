using Application.Features.HakedisPeriods.Constants;
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
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisPeriods.Queries.GetListByDynamic;

public class GetListByDynamicHakedisPeriodQuery : IRequest<GetListResponse<GetListByDynamicHakedisPeriodListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicHakedisPeriodQueryHandler : IRequestHandler<GetListByDynamicHakedisPeriodQuery, GetListResponse<GetListByDynamicHakedisPeriodListItemDto>>
    {
        private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicHakedisPeriodQueryHandler(IHakedisPeriodRepository hakedisPeriodRepository, IMapper mapper)
        {
            _hakedisPeriodRepository = hakedisPeriodRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicHakedisPeriodListItemDto>> Handle(GetListByDynamicHakedisPeriodQuery request, CancellationToken cancellationToken)
        {
            IPaginate<HakedisPeriod> hakedisPeriods = await _hakedisPeriodRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicHakedisPeriodListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicHakedisPeriodListItemDto>>(hakedisPeriods);
            return response;
        }
    }
}
