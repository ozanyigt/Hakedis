using NArchitecture.Core.Application.Dtos;
using Domain.Enums;
namespace Application.Features.ContractItems.Queries.GetListByDynamic;

public class GetListByDynamicContractItemListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? ContractQuantity { get; set; }
    public int SortOrder { get; set; }
}
