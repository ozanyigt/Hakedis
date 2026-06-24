using FluentValidation;

namespace Application.Features.MetrajRuleTemplates.Commands.Create;

public class CreateMetrajRuleTemplateCommandValidator : AbstractValidator<CreateMetrajRuleTemplateCommand>
{
    public CreateMetrajRuleTemplateCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.KalemType).NotEmpty();
        RuleFor(c => c.LayerPatterns).NotEmpty();
        RuleFor(c => c.EntityTypes).NotEmpty();
        RuleFor(c => c.Unit).IsInEnum();
        RuleFor(c => c.DeductOpenings).NotEmpty();
        RuleFor(c => c.IsDefault).NotEmpty();
        RuleFor(c => c.IsActive).NotEmpty();
    }
}