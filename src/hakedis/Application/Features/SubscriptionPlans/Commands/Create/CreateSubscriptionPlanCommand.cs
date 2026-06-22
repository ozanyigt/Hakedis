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

namespace Application.Features.SubscriptionPlans.Commands.Create;

public class CreateSubscriptionPlanCommand : IRequest<CreatedSubscriptionPlanResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal MonthlyPrice { get; set; }
    public required decimal YearlyPrice { get; set; }
    public required string EnabledModules { get; set; }
    public required int MaxSiteCount { get; set; }
    public required bool IsActive { get; set; }

    public string[] Roles => [Admin, Write, SubscriptionPlansOperationClaims.Create];

    public class CreateSubscriptionPlanCommandHandler : IRequestHandler<CreateSubscriptionPlanCommand, CreatedSubscriptionPlanResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly SubscriptionPlanBusinessRules _subscriptionPlanBusinessRules;

        public CreateSubscriptionPlanCommandHandler(IMapper mapper, ISubscriptionPlanRepository subscriptionPlanRepository,
                                         SubscriptionPlanBusinessRules subscriptionPlanBusinessRules)
        {
            _mapper = mapper;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _subscriptionPlanBusinessRules = subscriptionPlanBusinessRules;
        }

        public async Task<CreatedSubscriptionPlanResponse> Handle(CreateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            SubscriptionPlan subscriptionPlan = _mapper.Map<SubscriptionPlan>(request);

            await _subscriptionPlanRepository.AddAsync(subscriptionPlan);

            CreatedSubscriptionPlanResponse response = _mapper.Map<CreatedSubscriptionPlanResponse>(subscriptionPlan);
            return response;
        }
    }
}