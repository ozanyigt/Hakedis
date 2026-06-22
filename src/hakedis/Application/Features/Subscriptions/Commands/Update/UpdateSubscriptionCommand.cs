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

namespace Application.Features.Subscriptions.Commands.Update;

public class UpdateSubscriptionCommand : IRequest<UpdatedSubscriptionResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid SubscriptionPlanId { get; set; }
    public required BillingCycle BillingCycle { get; set; }
    public required SubscriptionStatus Status { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required bool IsManualAssignment { get; set; }
    public string? Notes { get; set; }

    public string[] Roles => [Admin, Write, SubscriptionsOperationClaims.Update];

    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, UpdatedSubscriptionResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly SubscriptionBusinessRules _subscriptionBusinessRules;

        public UpdateSubscriptionCommandHandler(IMapper mapper, ISubscriptionRepository subscriptionRepository,
                                         SubscriptionBusinessRules subscriptionBusinessRules)
        {
            _mapper = mapper;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionBusinessRules = subscriptionBusinessRules;
        }

        public async Task<UpdatedSubscriptionResponse> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            Subscription? subscription = await _subscriptionRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            await _subscriptionBusinessRules.SubscriptionShouldExistWhenSelected(subscription);
            subscription = _mapper.Map(request, subscription);

            await _subscriptionRepository.UpdateAsync(subscription!);

            UpdatedSubscriptionResponse response = _mapper.Map<UpdatedSubscriptionResponse>(subscription);
            return response;
        }
    }
}