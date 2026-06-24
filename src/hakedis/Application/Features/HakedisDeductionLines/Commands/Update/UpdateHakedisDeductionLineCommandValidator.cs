using Domain.Enums;
using FluentValidation;

namespace Application.Features.HakedisDeductionLines.Commands.Update;

public class UpdateHakedisDeductionLineCommandValidator : AbstractValidator<UpdateHakedisDeductionLineCommand>
{
    public UpdateHakedisDeductionLineCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.HakedisPeriodId).NotEmpty();
        RuleFor(c => c.Category).IsInEnum();
        RuleFor(c => c.Description).NotEmpty().MaximumLength(256);
        RuleFor(c => c.Amount).GreaterThan(0);
    }
}
