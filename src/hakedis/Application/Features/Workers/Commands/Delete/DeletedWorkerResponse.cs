using NArchitecture.Core.Application.Responses;

namespace Application.Features.Workers.Commands.Delete;

public class DeletedWorkerResponse : IResponse
{
    public Guid Id { get; set; }
}