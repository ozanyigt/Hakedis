using NArchitecture.Core.Application.Dtos;

namespace Application.Features.Tenants.Queries.GetList;

public class GetListTenantListItemDto : IDto
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