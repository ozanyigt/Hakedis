using FluentValidation;

namespace Application.Features.MetrajResults.Commands.Update;

public class UpdateMetrajResultCommandValidator : AbstractValidator<UpdateMetrajResultCommand>
{
    public UpdateMetrajResultCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.DrawingId).NotEmpty();
        RuleFor(c => c.KalemType).NotEmpty();
        RuleFor(c => c.Unit).IsInEnum();
        RuleFor(c => c.Quantity).NotEmpty();
        RuleFor(c => c.CalculatedAt).NotEmpty();
    }
}