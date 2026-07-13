using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Material : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual Tenant Tenant { get; set; } = null!;
}

public class SiteStockBalance : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageUnitCost { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Site Site { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}

public class StockTransaction : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid MaterialId { get; set; }
    public StockMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal BalanceQuantityAfter { get; set; }
    public decimal AverageUnitCostAfter { get; set; }
    public StockReferenceType ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid? TransferId { get; set; }
    public string IdempotencyKey { get; set; } = null!;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public Guid PostedByUserId { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime PostedAt { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Site Site { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}

public class DailySiteReportMaterialLine : Entity<Guid>
{
    public Guid DailySiteReportId { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public decimal? PostedUnitCost { get; set; }
    public decimal? PostedTotalCost { get; set; }
    public virtual DailySiteReport DailySiteReport { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}

public class DailySiteReportWorkforceSnapshot : Entity<Guid>
{
    public Guid DailySiteReportId { get; set; }
    public Guid SourcePuantajRecordId { get; set; }
    public Guid? WorkerId { get; set; }
    public string WorkerName { get; set; } = null!;
    public string? Trade { get; set; }
    public WorkType WorkType { get; set; }
    public decimal DayCount { get; set; }
    public decimal OvertimeHours { get; set; }
    public PuantajStatus PuantajStatusAtCapture { get; set; }
    public Guid CaptureBatchId { get; set; }
    public DateTime CapturedAt { get; set; }
    public virtual DailySiteReport DailySiteReport { get; set; } = null!;
}
