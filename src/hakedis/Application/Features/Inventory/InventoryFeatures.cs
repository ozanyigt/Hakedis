using Application.Services.Inventory;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Inventory;

public sealed record MaterialDto(
    Guid Id, Guid TenantId, string Code, string Name, string Unit, string? Description, bool IsActive);
public sealed record StockBalanceDto(
    Guid SiteId, string SiteName, Guid MaterialId, string MaterialCode, string MaterialName,
    string Unit, decimal Quantity, decimal AverageUnitCost, decimal TotalValue, string RowVersion);
public sealed record StockTransactionDto(
    Guid Id, Guid SiteId, string SiteName, Guid MaterialId, string MaterialCode,
    string MaterialName, string Unit,
    StockMovementType MovementType, decimal Quantity, decimal UnitCost, decimal TotalCost,
    decimal BalanceQuantityAfter, decimal AverageUnitCostAfter, StockReferenceType ReferenceType,
    Guid? ReferenceId, string IdempotencyKey, DateTime OccurredAt, DateTime PostedAt,
    string? Reference, string? Notes);

public class CreateMaterialCommand : IRequest<MaterialDto>, ISecuredRequest, ITransactionalRequest
{
    public Guid? TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string? Description { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Write, InventoryOperationClaims.Create];

    public class Handler(IMaterialRepository repository, InventoryBusinessRules rules)
        : IRequestHandler<CreateMaterialCommand, MaterialDto>
    {
        public async Task<MaterialDto> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
        {
            Guid tenantId = (await rules.ResolveTenantAsync(request.TenantId, true, cancellationToken))!.Value;
            string code = Required(request.Code, "Material code").ToUpperInvariant();
            if (await repository.AnyAsync(x => x.TenantId == tenantId && x.Code == code,
                    cancellationToken: cancellationToken))
                throw new BusinessException("Material code already exists in the tenant.");
            Material material = new()
            {
                TenantId = tenantId, Code = code, Name = Required(request.Name, "Material name"),
                Unit = Required(request.Unit, "Material unit"), Description = request.Description?.Trim()
            };
            await repository.AddAsync(material);
            return Map(material);
        }
    }

    internal static string Required(string? value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new BusinessException($"{name} is required.") : value.Trim();
    internal static MaterialDto Map(Material x) =>
        new(x.Id, x.TenantId, x.Code, x.Name, x.Unit, x.Description, x.IsActive);
}

public class UpdateMaterialCommand : IRequest<MaterialDto>, ISecuredRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Write, InventoryOperationClaims.Update];

    public class Handler(IMaterialRepository repository, InventoryBusinessRules rules)
        : IRequestHandler<UpdateMaterialCommand, MaterialDto>
    {
        public async Task<MaterialDto> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
        {
            Material material = await rules.GetMaterialAsync(request.Id, request.TenantId, cancellationToken);
            string code = CreateMaterialCommand.Required(request.Code, "Material code").ToUpperInvariant();
            if (await repository.AnyAsync(x => x.TenantId == material.TenantId && x.Code == code && x.Id != material.Id,
                    cancellationToken: cancellationToken))
                throw new BusinessException("Material code already exists in the tenant.");
            material.Code = code;
            material.Name = CreateMaterialCommand.Required(request.Name, "Material name");
            material.Unit = CreateMaterialCommand.Required(request.Unit, "Material unit");
            material.Description = request.Description?.Trim();
            material.IsActive = request.IsActive;
            await repository.UpdateAsync(material);
            return CreateMaterialCommand.Map(material);
        }
    }
}

public class DeleteMaterialCommand : IRequest<Guid>, ISecuredRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Write, InventoryOperationClaims.Delete];

    public class Handler(
        IMaterialRepository repository, ISiteStockBalanceRepository balances,
        IStockTransactionRepository transactions, IDailySiteReportMaterialLineRepository reportLines,
        InventoryBusinessRules rules)
        : IRequestHandler<DeleteMaterialCommand, Guid>
    {
        public async Task<Guid> Handle(DeleteMaterialCommand request, CancellationToken cancellationToken)
        {
            Material material = await rules.GetMaterialAsync(request.Id, request.TenantId, cancellationToken);
            if (await balances.AnyAsync(x => x.MaterialId == material.Id, cancellationToken: cancellationToken)
                || await transactions.AnyAsync(x => x.MaterialId == material.Id, cancellationToken: cancellationToken)
                || await reportLines.AnyAsync(x => x.MaterialId == material.Id, cancellationToken: cancellationToken))
                throw new BusinessException("A material with stock history cannot be deleted; deactivate it instead.");
            await repository.DeleteAsync(material);
            return material.Id;
        }
    }
}

public class GetMaterialsQuery : IRequest<IReadOnlyList<MaterialDto>>, ISecuredRequest
{
    public Guid? TenantId { get; set; }
    public bool IncludeInactive { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Read];
    public class Handler(IMaterialRepository repository, InventoryBusinessRules rules)
        : IRequestHandler<GetMaterialsQuery, IReadOnlyList<MaterialDto>>
    {
        public async Task<IReadOnlyList<MaterialDto>> Handle(GetMaterialsQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            IPaginate<Material> page = await repository.GetListAsync(
                x => (!tenantId.HasValue || x.TenantId == tenantId.Value) && (request.IncludeInactive || x.IsActive),
                orderBy: q => q.OrderBy(x => x.Name), size: 1000, enableTracking: false,
                cancellationToken: cancellationToken);
            return page.Items.Select(CreateMaterialCommand.Map).ToList();
        }
    }
}

public abstract class PostStockCommand : IRequest<StockPostingResult>, ISecuredRequest, ITransactionalRequest
{
    public Guid? TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public string IdempotencyKey { get; set; } = null!;
    public DateTime? OccurredAt { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public abstract StockMovementType MovementType { get; }
    public abstract decimal? UnitCost { get; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Write];
}

public class ReceiveStockCommand : PostStockCommand
{
    public decimal ReceiptUnitCost { get; set; }
    public override StockMovementType MovementType => StockMovementType.Receipt;
    public override decimal? UnitCost => ReceiptUnitCost;
    public class Handler(IStockPostingService posting, InventoryBusinessRules rules)
        : StockCommandHandler<ReceiveStockCommand>(posting, rules);
}

public class ConsumeStockCommand : PostStockCommand
{
    public override StockMovementType MovementType => StockMovementType.Consumption;
    public override decimal? UnitCost => null;
    public class Handler(IStockPostingService posting, InventoryBusinessRules rules)
        : StockCommandHandler<ConsumeStockCommand>(posting, rules);
}

public class AdjustStockCommand : PostStockCommand
{
    public bool Increase { get; set; }
    public decimal? AdjustmentUnitCost { get; set; }
    public override StockMovementType MovementType =>
        Increase ? StockMovementType.AdjustmentIncrease : StockMovementType.AdjustmentDecrease;
    public override decimal? UnitCost => AdjustmentUnitCost;
    public class Handler(IStockPostingService posting, InventoryBusinessRules rules)
        : StockCommandHandler<AdjustStockCommand>(posting, rules);
}

public abstract class StockCommandHandler<TCommand>(IStockPostingService posting, InventoryBusinessRules rules)
    : IRequestHandler<TCommand, StockPostingResult> where TCommand : PostStockCommand
{
    public async Task<StockPostingResult> Handle(TCommand request, CancellationToken cancellationToken)
    {
        Guid tenantId = (await rules.ResolveTenantAsync(request.TenantId, true, cancellationToken))!.Value;
        await rules.ValidateSiteAsync(tenantId, request.SiteId, null, cancellationToken);
        string key = CreateMaterialCommand.Required(request.IdempotencyKey, "Idempotency key");
        return await posting.PostAsync(new(
            tenantId, request.SiteId, request.MaterialId, request.MovementType, request.Quantity,
            request.UnitCost, StockReferenceType.Manual, null, null, key, request.Notes, rules.UserId,
            request.OccurredAt, request.Reference),
            cancellationToken);
    }
}

public class TransferStockCommand : IRequest<StockPostingResult>, ISecuredRequest, ITransactionalRequest
{
    public Guid? TenantId { get; set; }
    public Guid SourceSiteId { get; set; }
    public Guid DestinationSiteId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public string IdempotencyKey { get; set; } = null!;
    public DateTime? OccurredAt { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Write];
    public class Handler(IStockPostingService posting, InventoryBusinessRules rules)
        : IRequestHandler<TransferStockCommand, StockPostingResult>
    {
        public async Task<StockPostingResult> Handle(TransferStockCommand request, CancellationToken cancellationToken)
        {
            Guid tenantId = (await rules.ResolveTenantAsync(request.TenantId, true, cancellationToken))!.Value;
            await rules.ValidateSiteAsync(tenantId, request.SourceSiteId, null, cancellationToken);
            await rules.ValidateSiteAsync(tenantId, request.DestinationSiteId, null, cancellationToken);
            var result = await posting.TransferAsync(
                tenantId, request.SourceSiteId, request.DestinationSiteId, request.MaterialId,
                request.Quantity, CreateMaterialCommand.Required(request.IdempotencyKey, "Idempotency key"),
                request.Notes, rules.UserId, request.OccurredAt, request.Reference, cancellationToken);
            return result.Inbound;
        }
    }
}

public class GetStockBalancesQuery : IRequest<IReadOnlyList<StockBalanceDto>>, ISecuredRequest
{
    public Guid? TenantId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? MaterialId { get; set; }
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Read];
    public class Handler(ISiteStockBalanceRepository repository, InventoryBusinessRules rules)
        : IRequestHandler<GetStockBalancesQuery, IReadOnlyList<StockBalanceDto>>
    {
        public async Task<IReadOnlyList<StockBalanceDto>> Handle(
            GetStockBalancesQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            IPaginate<SiteStockBalance> page = await repository.GetListAsync(
                x => (!tenantId.HasValue || x.TenantId == tenantId.Value)
                    && (!request.SiteId.HasValue || x.SiteId == request.SiteId)
                    && (!request.MaterialId.HasValue || x.MaterialId == request.MaterialId),
                include: q => q.Include(x => x.Site).Include(x => x.Material),
                orderBy: q => q.OrderBy(x => x.Site.Name).ThenBy(x => x.Material.Name),
                size: 5000, enableTracking: false, cancellationToken: cancellationToken);
            return page.Items.Select(x => new StockBalanceDto(
                x.SiteId, x.Site.Name, x.MaterialId, x.Material.Code, x.Material.Name, x.Material.Unit,
                x.Quantity, x.AverageUnitCost, x.Quantity * x.AverageUnitCost,
                Convert.ToBase64String(x.RowVersion))).ToList();
        }
    }
}

public class GetStockLedgerQuery : IRequest<IReadOnlyList<StockTransactionDto>>, ISecuredRequest
{
    public Guid? TenantId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? MaterialId { get; set; }
    public int Take { get; set; } = 200;
    public string[] Roles => [InventoryOperationClaims.Admin, InventoryOperationClaims.Read];
    public class Handler(IStockTransactionRepository repository, InventoryBusinessRules rules)
        : IRequestHandler<GetStockLedgerQuery, IReadOnlyList<StockTransactionDto>>
    {
        public async Task<IReadOnlyList<StockTransactionDto>> Handle(
            GetStockLedgerQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = await rules.ResolveTenantAsync(request.TenantId, false, cancellationToken);
            IPaginate<StockTransaction> page = await repository.GetListAsync(
                x => (!tenantId.HasValue || x.TenantId == tenantId.Value)
                    && (!request.SiteId.HasValue || x.SiteId == request.SiteId)
                    && (!request.MaterialId.HasValue || x.MaterialId == request.MaterialId),
                include: q => q.Include(x => x.Site).Include(x => x.Material),
                orderBy: q => q.OrderByDescending(x => x.PostedAt), size: Math.Clamp(request.Take, 1, 1000),
                enableTracking: false, cancellationToken: cancellationToken);
            return page.Items.Select(x => new StockTransactionDto(
                x.Id, x.SiteId, x.Site.Name, x.MaterialId, x.Material.Code,
                x.Material.Name, x.Material.Unit, x.MovementType,
                x.Quantity, x.UnitCost, x.TotalCost, x.BalanceQuantityAfter, x.AverageUnitCostAfter,
                x.ReferenceType, x.ReferenceId, x.IdempotencyKey, x.OccurredAt, x.PostedAt,
                x.Reference, x.Notes)).ToList();
        }
    }
}
