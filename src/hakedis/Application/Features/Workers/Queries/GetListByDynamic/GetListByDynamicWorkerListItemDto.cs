using NArchitecture.Core.Application.Dtos;

namespace Application.Features.Workers.Queries.GetListByDynamic;

public class GetListByDynamicWorkerListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FullName { get; set; }
    public string? Trade { get; set; }
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public bool IsActive { get; set; }
}
