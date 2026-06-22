using FluentValidation;

namespace Application.Features.ContractItems.Commands.Delete;

public class DeleteContractItemCommandValidator : AbstractValidator<DeleteContractItemCommand>
{
    public DeleteContractItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}