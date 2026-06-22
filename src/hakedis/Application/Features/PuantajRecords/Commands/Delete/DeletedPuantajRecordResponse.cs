using NArchitecture.Core.Application.Responses;

namespace Application.Features.PuantajRecords.Commands.Delete;

public class DeletedPuantajRecordResponse : IResponse
{
    public Guid Id { get; set; }
}