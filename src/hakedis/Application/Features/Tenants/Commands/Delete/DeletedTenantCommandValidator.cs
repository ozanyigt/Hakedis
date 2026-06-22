using FluentValidation;

namespace Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}