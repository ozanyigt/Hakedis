using FluentValidation;

namespace Application.Features.Workers.Commands.Update;

public class UpdateWorkerCommandValidator : AbstractValidator<UpdateWorkerCommand>
{
    public UpdateWorkerCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.FullName).NotEmpty();
        RuleFor(c => c.IsActive).NotEmpty();
    }
}