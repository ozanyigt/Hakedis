using FluentValidation;

namespace Application.Features.HakedisPeriods.Commands.Delete;

public class DeleteHakedisPeriodCommandValidator : AbstractValidator<DeleteHakedisPeriodCommand>
{
    public DeleteHakedisPeriodCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}