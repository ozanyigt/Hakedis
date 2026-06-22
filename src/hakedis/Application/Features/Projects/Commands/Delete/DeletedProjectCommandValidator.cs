using FluentValidation;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}