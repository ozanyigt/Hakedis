using FluentValidation;

namespace Application.Features.Workers.Commands.Delete;

public class DeleteWorkerCommandValidator : AbstractValidator<DeleteWorkerCommand>
{
    public DeleteWorkerCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}