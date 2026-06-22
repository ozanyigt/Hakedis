using Application.Features.SubscriptionPlans.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.SubscriptionPlans.Constants.SubscriptionPlansOperationClaims;

namespace Application.Features.SubscriptionPlans.Queries.GetList;

public class GetListSubscriptionPlanQuery : IRequest<GetListResponse<GetListSubscriptionPlanListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListSubscriptionPlanQueryHandler : IRequestHandler<GetListSubscriptionPlanQuery, GetListResponse<GetListSubscriptionPlanListItemDto>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;

        public GetListSubscriptionPlanQueryHandler(ISubscriptionPlanRepository subscriptionPlanRepository, IMapper mapper)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListSubscriptionPlanListItemDto>> Handle(GetListSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            IPaginate<SubscriptionPlan> subscriptionPlans = await _subscriptionPlanRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListSubscriptionPlanListItemDto> response = _mapper.Map<GetListResponse<GetListSubscriptionPlanListItemDto>>(subscriptionPlans);
            return response;
        }
    }
}