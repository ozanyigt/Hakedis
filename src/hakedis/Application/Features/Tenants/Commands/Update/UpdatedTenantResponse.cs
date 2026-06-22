using NArchitecture.Core.Application.Responses;

namespace Application.Features.Tenants.Commands.Update;

public class UpdatedTenantResponse : IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}