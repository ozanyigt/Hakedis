using FluentValidation;

namespace Application.Features.Drawings.Commands.Create;

public class CreateDrawingCommandValidator : AbstractValidator<CreateDrawingCommand>
{
    public CreateDrawingCommandValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
        RuleFor(c => c.FilePath).NotEmpty();
        RuleFor(c => c.FileExtension).NotEmpty();
        RuleFor(c => c.FileSizeBytes).NotEmpty();
        RuleFor(c => c.Status).NotEmpty();
    }
}