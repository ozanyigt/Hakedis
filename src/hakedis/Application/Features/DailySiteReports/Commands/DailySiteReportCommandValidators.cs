using FluentValidation;

namespace Application.Features.DailySiteReports.Commands;

public abstract class DailySiteReportWriteModelValidator<T> : AbstractValidator<T>
    where T : DailySiteReportWriteModel
{
    protected DailySiteReportWriteModelValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.ReportDate).NotEmpty();
        RuleFor(x => x.Weather).IsInEnum();
        RuleFor(x => x.WorkSummary).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.WorkforceNotes).MaximumLength(4000);
        RuleFor(x => x.EquipmentNotes).MaximumLength(4000);
        RuleFor(x => x.MaterialNotes).MaximumLength(4000);
        RuleFor(x => x.BlockersNotes).MaximumLength(4000);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.MinTemperatureCelsius).InclusiveBetween(-100, 100).When(x => x.MinTemperatureCelsius.HasValue);
        RuleFor(x => x.MaxTemperatureCelsius).InclusiveBetween(-100, 100).When(x => x.MaxTemperatureCelsius.HasValue);
    }
}

public class CreateDailySiteReportCommandValidator : DailySiteReportWriteModelValidator<CreateDailySiteReportCommand>;

public class UpdateDailySiteReportCommandValidator : DailySiteReportWriteModelValidator<UpdateDailySiteReportCommand>
{
    public UpdateDailySiteReportCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public class RejectDailySiteReportCommandValidator : AbstractValidator<RejectDailySiteReportCommand>
{
    public RejectDailySiteReportCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}

public class UploadDailySiteReportPhotoCommandValidator : AbstractValidator<UploadDailySiteReportPhotoCommand>
{
    public UploadDailySiteReportPhotoCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
