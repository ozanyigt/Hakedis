using Application.Features.Subscriptions.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Subscriptions.Constants.SubscriptionsOperationClaims;

namespace Application.Features.Subscriptions.Queries.GetList;

public class GetListSubscriptionQuery : IRequest<GetListResponse<GetListSubscriptionListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListSubscriptionQueryHandler : IRequestHandler<GetListSubscriptionQuery, GetListResponse<GetListSubscriptionListItemDto>>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;

        public GetListSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListSubscriptionListItemDto>> Handle(GetListSubscriptionQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Subscription> subscriptions = await _subscriptionRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListSubscriptionListItemDto> response = _mapper.Map<GetListResponse<GetListSubscriptionListItemDto>>(subscriptions);
            return response;
        }
    }
}