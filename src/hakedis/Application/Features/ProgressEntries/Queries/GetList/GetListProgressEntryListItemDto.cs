using NArchitecture.Core.Application.Dtos;

namespace Application.Features.ProgressEntries.Queries.GetList;

public class GetListProgressEntryListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public Guid ContractItemId { get; set; }
    public decimal QuantityThisPeriod { get; set; }
    public decimal CumulativeQuantity { get; set; }
    public decimal AmountThisPeriod { get; set; }
    public Guid? MetrajResultId { get; set; }
    public bool IsManualEntry { get; set; }
    public string? Notes { get; set; }
}