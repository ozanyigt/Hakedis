using FluentValidation;

namespace Application.Features.Subscriptions.Commands.Create;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.SubscriptionPlanId).NotEmpty();
        RuleFor(c => c.BillingCycle).NotEmpty();
        RuleFor(c => c.Status).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.IsManualAssignment).NotEmpty();
    }
}