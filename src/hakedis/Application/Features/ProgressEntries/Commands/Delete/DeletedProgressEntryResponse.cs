using NArchitecture.Core.Application.Responses;

namespace Application.Features.ProgressEntries.Commands.Delete;

public class DeletedProgressEntryResponse : IResponse
{
    public Guid Id { get; set; }
}