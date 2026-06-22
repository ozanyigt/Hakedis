using FluentValidation;

namespace Application.Features.MetrajRuleTemplates.Commands.Delete;

public class DeleteMetrajRuleTemplateCommandValidator : AbstractValidator<DeleteMetrajRuleTemplateCommand>
{
    public DeleteMetrajRuleTemplateCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}