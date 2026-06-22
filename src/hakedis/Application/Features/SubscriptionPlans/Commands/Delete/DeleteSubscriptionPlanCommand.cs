using Application.Features.SubscriptionPlans.Constants;
using Application.Features.SubscriptionPlans.Constants;
using Application.Features.SubscriptionPlans.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.SubscriptionPlans.Constants.SubscriptionPlansOperationClaims;

namespace Application.Features.SubscriptionPlans.Commands.Delete;

public class DeleteSubscriptionPlanCommand : IRequest<DeletedSubscriptionPlanResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, SubscriptionPlansOperationClaims.Delete];

    public class DeleteSubscriptionPlanCommandHandler : IRequestHandler<DeleteSubscriptionPlanCommand, DeletedSubscriptionPlanResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly SubscriptionPlanBusinessRules _subscriptionPlanBusinessRules;

        public DeleteSubscriptionPlanCommandHandler(IMapper mapper, ISubscriptionPlanRepository subscriptionPlanRepository,
                                         SubscriptionPlanBusinessRules subscriptionPlanBusinessRules)
        {
            _mapper = mapper;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _subscriptionPlanBusinessRules = subscriptionPlanBusinessRules;
        }

        public async Task<DeletedSubscriptionPlanResponse> Handle(DeleteSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetAsync(predicate: sp => sp.Id == request.Id, cancellationToken: cancellationToken);
            await _subscriptionPlanBusinessRules.SubscriptionPlanShouldExistWhenSelected(subscriptionPlan);

            await _subscriptionPlanRepository.DeleteAsync(subscriptionPlan!);

            DeletedSubscriptionPlanResponse response = _mapper.Map<DeletedSubscriptionPlanResponse>(subscriptionPlan);
            return response;
        }
    }
}