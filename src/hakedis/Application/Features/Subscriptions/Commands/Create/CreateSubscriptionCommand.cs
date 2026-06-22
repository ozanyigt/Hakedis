using Application.Features.Subscriptions.Constants;
using Application.Features.Subscriptions.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using Domain.Enums;
using static Application.Features.Subscriptions.Constants.SubscriptionsOperationClaims;

namespace Application.Features.Subscriptions.Commands.Create;

public class CreateSubscriptionCommand : IRequest<CreatedSubscriptionResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid SubscriptionPlanId { get; set; }
    public required BillingCycle BillingCycle { get; set; }
    public required SubscriptionStatus Status { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required bool IsManualAssignment { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, SubscriptionsOperationClaims.Create];

    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, CreatedSubscriptionResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly SubscriptionBusinessRules _subscriptionBusinessRules;

        public CreateSubscriptionCommandHandler(IMapper mapper, ISubscriptionRepository subscriptionRepository,
                                         SubscriptionBusinessRules subscriptionBusinessRules)
        {
            _mapper = mapper;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionBusinessRules = subscriptionBusinessRules;
        }

        public async Task<CreatedSubscriptionResponse> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            Subscription subscription = _mapper.Map<Subscription>(request);

            await _subscriptionRepository.AddAsync(subscription);

            CreatedSubscriptionResponse response = _mapper.Map<CreatedSubscriptionResponse>(subscription);
            return response;
        }
    }
}