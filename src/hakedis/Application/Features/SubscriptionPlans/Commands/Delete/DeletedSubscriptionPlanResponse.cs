using NArchitecture.Core.Application.Responses;

namespace Application.Features.SubscriptionPlans.Commands.Delete;

public class DeletedSubscriptionPlanResponse : IResponse
{
    public Guid Id { get; set; }
}