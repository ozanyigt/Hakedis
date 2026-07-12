using FluentValidation;

namespace Application.Features.PuantajRecords.Commands.Update;

public class UpdatePuantajRecordCommandValidator : AbstractValidator<UpdatePuantajRecordCommand>
{
    public UpdatePuantajRecordCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.WorkDate).NotEmpty();
        RuleFor(c => c.WorkType).IsInEnum();
        RuleFor(c => c.DayCount).GreaterThan(0);
        RuleFor(c => c.OvertimeHours).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Status).IsInEnum();
    }
}