using System.Transactions;
using Application.Services.Inventory;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using Persistence.Contexts;

namespace Persistence.Services;

public sealed class StockPostingService(BaseDbContext context) : IStockPostingService
{
    public async Task<StockPostingResult> PostAsync(
        StockPostingRequest request, CancellationToken cancellationToken)
    {
        IDbContextTransaction? transaction = await BeginLocalTransactionAsync(cancellationToken);
        try
        {
            StockPostingResult result = await PostCoreAsync(request, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw new BusinessException("Stock changed concurrently. Retry the operation.");
        }
        catch
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    public async Task<(StockPostingResult Outbound, StockPostingResult Inbound)> TransferAsync(
        Guid tenantId, Guid sourceSiteId, Guid destinationSiteId, Guid materialId,
        decimal quantity, string idempotencyKey, string? notes, Guid postedByUserId,
        DateTime? occurredAt, string? reference,
        CancellationToken cancellationToken)
    {
        if (sourceSiteId == destinationSiteId)
            throw new BusinessException("Transfer sites must be different.");

        IDbContextTransaction? transaction = await BeginLocalTransactionAsync(cancellationToken);
        try
        {
            Guid transferId = Guid.NewGuid();
            StockPostingResult outbound = await PostCoreAsync(new(
                tenantId, sourceSiteId, materialId, StockMovementType.TransferOut, quantity, null,
                StockReferenceType.Transfer, transferId, transferId, $"{idempotencyKey}:out", notes,
                postedByUserId, occurredAt, reference), cancellationToken);
            StockPostingResult inbound = await PostCoreAsync(new(
                tenantId, destinationSiteId, materialId, StockMovementType.TransferIn, quantity,
                outbound.UnitCost, StockReferenceType.Transfer, transferId, transferId,
                $"{idempotencyKey}:in", notes, postedByUserId, occurredAt, reference), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);
            return (outbound, inbound);
        }
        catch
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    private async Task<StockPostingResult> PostCoreAsync(
        StockPostingRequest request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new BusinessException("Stock quantity must be greater than zero.");
        StockTransaction? existing = await context.StockTransactions.AsNoTracking().SingleOrDefaultAsync(
            x => x.TenantId == request.TenantId && x.IdempotencyKey == request.IdempotencyKey,
            cancellationToken);
        if (existing is not null)
            return ToResult(existing);

        bool validMaterial = await context.Materials.AnyAsync(
            x => x.Id == request.MaterialId && x.TenantId == request.TenantId && x.IsActive,
            cancellationToken);
        bool validSite = await context.Sites.AnyAsync(
            x => x.Id == request.SiteId && x.TenantId == request.TenantId, cancellationToken);
        if (!validMaterial || !validSite)
            throw new BusinessException("Material or site is outside the selected tenant.");

        SiteStockBalance? balance = await context.SiteStockBalances.SingleOrDefaultAsync(
            x => x.TenantId == request.TenantId && x.SiteId == request.SiteId
                 && x.MaterialId == request.MaterialId, cancellationToken);
        balance ??= new SiteStockBalance
        {
            TenantId = request.TenantId, SiteId = request.SiteId, MaterialId = request.MaterialId
        };
        if (balance.Id == Guid.Empty)
            context.SiteStockBalances.Add(balance);

        bool inbound = request.MovementType is StockMovementType.Receipt
            or StockMovementType.TransferIn or StockMovementType.AdjustmentIncrease;
        decimal unitCost;
        if (inbound)
        {
            if (!request.UnitCost.HasValue || request.UnitCost.Value < 0)
                throw new BusinessException("An inbound stock movement requires a non-negative unit cost.");
            unitCost = request.UnitCost.Value;
            (balance.Quantity, balance.AverageUnitCost) = StockValuationCalculator.Receive(
                balance.Quantity, balance.AverageUnitCost, request.Quantity, unitCost);
        }
        else
        {
            (balance.Quantity, balance.AverageUnitCost, unitCost) = StockValuationCalculator.Issue(
                balance.Quantity, balance.AverageUnitCost, request.Quantity);
        }

        StockTransaction stockTransaction = new()
        {
            TenantId = request.TenantId,
            SiteId = request.SiteId,
            MaterialId = request.MaterialId,
            MovementType = request.MovementType,
            Quantity = request.Quantity,
            UnitCost = unitCost,
            TotalCost = request.Quantity * unitCost,
            BalanceQuantityAfter = balance.Quantity,
            AverageUnitCostAfter = balance.AverageUnitCost,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            TransferId = request.TransferId,
            IdempotencyKey = request.IdempotencyKey.Trim(),
            Reference = request.Reference?.Trim(),
            Notes = request.Notes?.Trim(),
            PostedByUserId = request.PostedByUserId,
            OccurredAt = request.OccurredAt?.ToUniversalTime() ?? DateTime.UtcNow,
            PostedAt = DateTime.UtcNow
        };
        context.StockTransactions.Add(stockTransaction);
        return ToResult(stockTransaction);
    }

    private async Task<IDbContextTransaction?> BeginLocalTransactionAsync(CancellationToken cancellationToken)
    {
        if (context.Database.CurrentTransaction is not null || Transaction.Current is not null)
            return null;
        return await context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
    }

    private static StockPostingResult ToResult(StockTransaction transaction) =>
        new(transaction.Id, transaction.UnitCost, transaction.TotalCost,
            transaction.BalanceQuantityAfter, transaction.AverageUnitCostAfter);
}
