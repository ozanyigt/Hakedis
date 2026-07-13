using Domain.Enums;

namespace Application.Services.Inventory;

public sealed record StockPostingRequest(
    Guid TenantId,
    Guid SiteId,
    Guid MaterialId,
    StockMovementType MovementType,
    decimal Quantity,
    decimal? UnitCost,
    StockReferenceType ReferenceType,
    Guid? ReferenceId,
    Guid? TransferId,
    string IdempotencyKey,
    string? Notes,
    Guid PostedByUserId,
    DateTime? OccurredAt = null,
    string? Reference = null);

public sealed record StockPostingResult(
    Guid TransactionId,
    decimal UnitCost,
    decimal TotalCost,
    decimal BalanceQuantity,
    decimal AverageUnitCost);

public interface IStockPostingService
{
    Task<StockPostingResult> PostAsync(StockPostingRequest request, CancellationToken cancellationToken);
    Task<(StockPostingResult Outbound, StockPostingResult Inbound)> TransferAsync(
        Guid tenantId, Guid sourceSiteId, Guid destinationSiteId, Guid materialId,
        decimal quantity, string idempotencyKey, string? notes, Guid postedByUserId,
        DateTime? occurredAt, string? reference,
        CancellationToken cancellationToken);
}
