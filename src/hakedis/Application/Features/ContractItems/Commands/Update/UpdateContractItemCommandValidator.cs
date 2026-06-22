using FluentValidation;

namespace Application.Features.ContractItems.Commands.Update;

public class UpdateContractItemCommandValidator : AbstractValidator<UpdateContractItemCommand>
{
    public UpdateContractItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.KalemType).NotEmpty();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Unit).NotEmpty();
        RuleFor(c => c.UnitPrice).NotEmpty();
        RuleFor(c => c.SortOrder).NotEmpty();
    }
}