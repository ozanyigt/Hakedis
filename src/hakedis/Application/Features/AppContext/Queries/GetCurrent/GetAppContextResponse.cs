using NArchitecture.Core.Application.Responses;

namespace Application.Features.AppContext.Queries.GetCurrent;

public class GetAppContextResponse : IResponse
{
    public bool IsGlobalAdmin { get; set; }
    public int? FirmRole { get; set; }
    public int? SecondaryFirmRole { get; set; }
    public Guid? TenantId { get; set; }    public string? TenantName { get; set; }
    public IReadOnlyList<string> EnabledModules { get; set; } = [];
    public bool CanSwitchTenant { get; set; }
    public IReadOnlyList<AppContextTenantItemDto> Tenants { get; set; } = [];
}

public class AppContextTenantItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
