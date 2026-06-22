using NArchitecture.Core.Application.Responses;

namespace Application.Features.Projects.Commands.Delete;

public class DeletedProjectResponse : IResponse
{
    public Guid Id { get; set; }
}