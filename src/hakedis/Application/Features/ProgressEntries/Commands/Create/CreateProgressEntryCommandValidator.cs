using FluentValidation;

namespace Application.Features.ProgressEntries.Commands.Create;

public class CreateProgressEntryCommandValidator : AbstractValidator<CreateProgressEntryCommand>
{
    public CreateProgressEntryCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.HakedisPeriodId).NotEmpty();
        RuleFor(c => c.ContractItemId).NotEmpty();
        RuleFor(c => c.QuantityThisPeriod).NotEmpty();
        RuleFor(c => c.CumulativeQuantity).NotEmpty();
        RuleFor(c => c.AmountThisPeriod).NotEmpty();
        RuleFor(c => c.IsManualEntry).NotEmpty();
    }
}