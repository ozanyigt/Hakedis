using FluentValidation;

namespace Application.Features.MetrajRuleTemplates.Commands.Update;

public class UpdateMetrajRuleTemplateCommandValidator : AbstractValidator<UpdateMetrajRuleTemplateCommand>
{
    public UpdateMetrajRuleTemplateCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.KalemType).NotEmpty();
        RuleFor(c => c.LayerPatterns).NotEmpty();
        RuleFor(c => c.EntityTypes).NotEmpty();
        RuleFor(c => c.Unit).NotEmpty();
        RuleFor(c => c.DeductOpenings).NotEmpty();
        RuleFor(c => c.IsDefault).NotEmpty();
        RuleFor(c => c.IsActive).NotEmpty();
    }
}