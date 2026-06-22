using FluentValidation;

namespace Application.Features.PuantajRecords.Commands.Delete;

public class DeletePuantajRecordCommandValidator : AbstractValidator<DeletePuantajRecordCommand>
{
    public DeletePuantajRecordCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}