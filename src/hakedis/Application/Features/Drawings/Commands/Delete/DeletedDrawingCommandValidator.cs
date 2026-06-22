using FluentValidation;

namespace Application.Features.Drawings.Commands.Delete;

public class DeleteDrawingCommandValidator : AbstractValidator<DeleteDrawingCommand>
{
    public DeleteDrawingCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}