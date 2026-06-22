using FluentValidation;

namespace Application.Features.SubscriptionPlans.Commands.Update;

public class UpdateSubscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
{
    public UpdateSubscriptionPlanCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.MonthlyPrice).NotEmpty();
        RuleFor(c => c.YearlyPrice).NotEmpty();
        RuleFor(c => c.EnabledModules).NotEmpty();
        RuleFor(c => c.MaxSiteCount).NotEmpty();
        RuleFor(c => c.IsActive).NotEmpty();
    }
}