using Domain;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.ContractItems.Commands.Update;

public class UpdateContractItemCommandValidator : AbstractValidator<UpdateContractItemCommand>
{
    public UpdateContractItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.KalemType).IsInEnum();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Unit)
            .IsInEnum()
            .Must((cmd, unit) => MeasurementUnitDefaults.GetAllowedForKalemType(cmd.KalemType).Contains(unit))
            .WithMessage("Seçilen birim bu kalem tipi için geçerli değil.");
        RuleFor(c => c.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(c => c.SortOrder).NotEmpty();
    }
}
