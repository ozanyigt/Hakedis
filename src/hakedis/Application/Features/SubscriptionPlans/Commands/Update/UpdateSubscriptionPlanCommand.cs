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

namespace Application.Features.SubscriptionPlans.Commands.Update;

public class UpdateSubscriptionPlanCommand : IRequest<UpdatedSubscriptionPlanResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal MonthlyPrice { get; set; }
    public required decimal YearlyPrice { get; set; }
    public required string EnabledModules { get; set; }
    public required int MaxSiteCount { get; set; }
    public required bool IsActive { get; set; }

    public string[] Roles => [Admin, Write, SubscriptionPlansOperationClaims.Update];

    public class UpdateSubscriptionPlanCommandHandler : IRequestHandler<UpdateSubscriptionPlanCommand, UpdatedSubscriptionPlanResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly SubscriptionPlanBusinessRules _subscriptionPlanBusinessRules;

        public UpdateSubscriptionPlanCommandHandler(IMapper mapper, ISubscriptionPlanRepository subscriptionPlanRepository,
                                         SubscriptionPlanBusinessRules subscriptionPlanBusinessRules)
        {
            _mapper = mapper;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _subscriptionPlanBusinessRules = subscriptionPlanBusinessRules;
        }

        public async Task<UpdatedSubscriptionPlanResponse> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetAsync(predicate: sp => sp.Id == request.Id, cancellationToken: cancellationToken);
            await _subscriptionPlanBusinessRules.SubscriptionPlanShouldExistWhenSelected(subscriptionPlan);
            subscriptionPlan = _mapper.Map(request, subscriptionPlan);

            await _subscriptionPlanRepository.UpdateAsync(subscriptionPlan!);

            UpdatedSubscriptionPlanResponse response = _mapper.Map<UpdatedSubscriptionPlanResponse>(subscriptionPlan);
            return response;
        }
    }
}