using NArchitecture.Core.Application.Responses;

namespace Application.Features.Drawings.Commands.Delete;

public class DeletedDrawingResponse : IResponse
{
    public Guid Id { get; set; }
}