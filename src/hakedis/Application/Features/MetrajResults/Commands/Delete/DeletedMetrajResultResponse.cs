using NArchitecture.Core.Application.Responses;

namespace Application.Features.MetrajResults.Commands.Delete;

public class DeletedMetrajResultResponse : IResponse
{
    public Guid Id { get; set; }
}