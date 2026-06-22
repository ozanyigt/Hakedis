using Application.Features.SubscriptionPlans.Constants;
using Application.Features.SubscriptionPlans.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.SubscriptionPlans.Constants.SubscriptionPlansOperationClaims;

namespace Application.Features.SubscriptionPlans.Queries.GetById;

public class GetByIdSubscriptionPlanQuery : IRequest<GetByIdSubscriptionPlanResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdSubscriptionPlanQueryHandler : IRequestHandler<GetByIdSubscriptionPlanQuery, GetByIdSubscriptionPlanResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly SubscriptionPlanBusinessRules _subscriptionPlanBusinessRules;

        public GetByIdSubscriptionPlanQueryHandler(IMapper mapper, ISubscriptionPlanRepository subscriptionPlanRepository, SubscriptionPlanBusinessRules subscriptionPlanBusinessRules)
        {
            _mapper = mapper;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _subscriptionPlanBusinessRules = subscriptionPlanBusinessRules;
        }

        public async Task<GetByIdSubscriptionPlanResponse> Handle(GetByIdSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetAsync(predicate: sp => sp.Id == request.Id, cancellationToken: cancellationToken);
            await _subscriptionPlanBusinessRules.SubscriptionPlanShouldExistWhenSelected(subscriptionPlan);

            GetByIdSubscriptionPlanResponse response = _mapper.Map<GetByIdSubscriptionPlanResponse>(subscriptionPlan);
            return response;
        }
    }
}