using NArchitecture.Core.Application.Responses;
using Domain.Enums;

namespace Application.Features.ContractItems.Commands.Update;

public class UpdatedContractItemResponse : IResponse
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