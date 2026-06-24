using FluentValidation;

namespace Application.Features.ProgressEntries.Commands.Update;

public class UpdateProgressEntryCommandValidator : AbstractValidator<UpdateProgressEntryCommand>
{
    public UpdateProgressEntryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.HakedisPeriodId).NotEmpty();
        RuleFor(c => c.ContractItemId).NotEmpty();
        RuleFor(c => c.QuantityThisPeriod).GreaterThanOrEqualTo(0);
        RuleFor(c => c.CumulativeQuantity).GreaterThanOrEqualTo(0);
        RuleFor(c => c.AmountThisPeriod).GreaterThanOrEqualTo(0);
    }
}