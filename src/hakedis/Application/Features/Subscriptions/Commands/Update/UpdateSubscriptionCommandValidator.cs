using FluentValidation;

namespace Application.Features.Subscriptions.Commands.Update;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.SubscriptionPlanId).NotEmpty();
        RuleFor(c => c.BillingCycle).NotEmpty();
        RuleFor(c => c.Status).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.IsManualAssignment).NotEmpty();
    }
}