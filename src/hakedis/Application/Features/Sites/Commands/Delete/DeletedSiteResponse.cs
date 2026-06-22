using NArchitecture.Core.Application.Responses;

namespace Application.Features.Sites.Commands.Delete;

public class DeletedSiteResponse : IResponse
{
    public Guid Id { get; set; }
}