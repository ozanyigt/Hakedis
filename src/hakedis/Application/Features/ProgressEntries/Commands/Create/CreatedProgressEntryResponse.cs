using NArchitecture.Core.Application.Responses;

namespace Application.Features.ProgressEntries.Commands.Create;

public class CreatedProgressEntryResponse : IResponse
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