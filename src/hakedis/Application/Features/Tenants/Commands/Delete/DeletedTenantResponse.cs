using NArchitecture.Core.Application.Responses;

namespace Application.Features.Tenants.Commands.Delete;

public class DeletedTenantResponse : IResponse
{
    public Guid Id { get; set; }
}