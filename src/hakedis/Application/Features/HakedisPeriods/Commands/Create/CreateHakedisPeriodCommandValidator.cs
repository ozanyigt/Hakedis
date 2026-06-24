using FluentValidation;

namespace Application.Features.HakedisPeriods.Commands.Create;

public class CreateHakedisPeriodCommandValidator : AbstractValidator<CreateHakedisPeriodCommand>
{
    public CreateHakedisPeriodCommandValidator()
    {
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