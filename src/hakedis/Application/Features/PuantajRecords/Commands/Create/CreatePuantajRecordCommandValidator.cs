using FluentValidation;

namespace Application.Features.PuantajRecords.Commands.Create;

public class CreatePuantajRecordCommandValidator : AbstractValidator<CreatePuantajRecordCommand>
{
    public CreatePuantajRecordCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.WorkDate).NotEmpty();
        RuleFor(c => c.WorkType).NotEmpty();
        RuleFor(c => c.DayCount).NotEmpty();
        RuleFor(c => c.OvertimeHours).NotEmpty();
        RuleFor(c => c.Status).NotEmpty();
    }
}