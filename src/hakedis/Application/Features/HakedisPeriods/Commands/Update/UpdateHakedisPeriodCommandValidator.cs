using FluentValidation;

namespace Application.Features.HakedisPeriods.Commands.Update;

public class UpdateHakedisPeriodCommandValidator : AbstractValidator<UpdateHakedisPeriodCommand>
{
    public UpdateHakedisPeriodCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.PeriodNumber).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.PeriodStart).NotEmpty();
        RuleFor(c => c.PeriodEnd).NotEmpty();
        RuleFor(c => c.Status).NotEmpty();
        RuleFor(c => c.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(c => c.DeductionAmount).GreaterThanOrEqualTo(0);
    }
}