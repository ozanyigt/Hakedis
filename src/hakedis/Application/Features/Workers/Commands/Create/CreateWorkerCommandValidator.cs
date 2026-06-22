using FluentValidation;

namespace Application.Features.Workers.Commands.Create;

public class CreateWorkerCommandValidator : AbstractValidator<CreateWorkerCommand>
{
    public CreateWorkerCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.FullName).NotEmpty();
        RuleFor(c => c.IsActive).NotEmpty();
    }
}