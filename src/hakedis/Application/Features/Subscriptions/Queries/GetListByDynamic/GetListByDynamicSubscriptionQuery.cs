using Application.Features.Subscriptions.Constants;
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
using static Application.Features.Subscriptions.Constants.SubscriptionsOperationClaims;

namespace Application.Features.Subscriptions.Queries.GetListByDynamic;

public class GetListByDynamicSubscriptionQuery : IRequest<GetListResponse<GetListByDynamicSubscriptionListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicSubscriptionQueryHandler : IRequestHandler<GetListByDynamicSubscriptionQuery, GetListResponse<GetListByDynamicSubscriptionListItemDto>>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicSubscriptionListItemDto>> Handle(GetListByDynamicSubscriptionQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Subscription> subscriptions = await _subscriptionRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicSubscriptionListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicSubscriptionListItemDto>>(subscriptions);
            return response;
        }
    }
}
