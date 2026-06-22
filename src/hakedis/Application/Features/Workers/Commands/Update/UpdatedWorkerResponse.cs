using NArchitecture.Core.Application.Responses;

namespace Application.Features.Workers.Commands.Update;

public class UpdatedWorkerResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FullName { get; set; }
    public string? Trade { get; set; }
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public bool IsActive { get; set; }
}