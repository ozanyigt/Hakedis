using FluentValidation;

namespace Application.Features.Sites.Commands.Delete;

public class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}