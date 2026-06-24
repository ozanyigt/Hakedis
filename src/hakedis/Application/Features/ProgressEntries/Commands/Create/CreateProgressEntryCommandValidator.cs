using FluentValidation;

namespace Application.Features.ProgressEntries.Commands.Create;

public class CreateProgressEntryCommandValidator : AbstractValidator<CreateProgressEntryCommand>
{
    public CreateProgressEntryCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.HakedisPeriodId).NotEmpty();
        RuleFor(c => c.ContractItemId).NotEmpty();
        RuleFor(c => c.QuantityThisPeriod).GreaterThanOrEqualTo(0);
        RuleFor(c => c.CumulativeQuantity).GreaterThanOrEqualTo(0);
        RuleFor(c => c.AmountThisPeriod).GreaterThanOrEqualTo(0);
    }
}