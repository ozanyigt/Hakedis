using Domain.Enums;
using FluentValidation;

namespace Application.Features.HakedisDeductionLines.Commands.Create;

public class CreateHakedisDeductionLineCommandValidator : AbstractValidator<CreateHakedisDeductionLineCommand>
{
    public CreateHakedisDeductionLineCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.HakedisPeriodId).NotEmpty();
        RuleFor(c => c.Category).IsInEnum();
        RuleFor(c => c.Description).NotEmpty().MaximumLength(256);
        RuleFor(c => c.Amount).GreaterThan(0);
    }
}
