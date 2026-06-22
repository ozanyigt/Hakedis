using FluentValidation;

namespace Application.Features.MetrajResults.Commands.Delete;

public class DeleteMetrajResultCommandValidator : AbstractValidator<DeleteMetrajResultCommand>
{
    public DeleteMetrajResultCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}