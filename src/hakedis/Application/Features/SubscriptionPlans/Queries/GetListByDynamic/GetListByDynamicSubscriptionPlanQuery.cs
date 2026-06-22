using Application.Features.SubscriptionPlans.Constants;
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
using static Application.Features.SubscriptionPlans.Constants.SubscriptionPlansOperationClaims;

namespace Application.Features.SubscriptionPlans.Queries.GetListByDynamic;

public class GetListByDynamicSubscriptionPlanQuery : IRequest<GetListResponse<GetListByDynamicSubscriptionPlanListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicSubscriptionPlanQueryHandler : IRequestHandler<GetListByDynamicSubscriptionPlanQuery, GetListResponse<GetListByDynamicSubscriptionPlanListItemDto>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicSubscriptionPlanQueryHandler(ISubscriptionPlanRepository subscriptionPlanRepository, IMapper mapper)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicSubscriptionPlanListItemDto>> Handle(GetListByDynamicSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            IPaginate<SubscriptionPlan> subscriptionPlans = await _subscriptionPlanRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicSubscriptionPlanListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicSubscriptionPlanListItemDto>>(subscriptionPlans);
            return response;
        }
    }
}
