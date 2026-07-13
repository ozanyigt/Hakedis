using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Application.Services.Inventory;

public static class StockValuationCalculator
{
    public static (decimal Quantity, decimal AverageUnitCost) Receive(
        decimal currentQuantity, decimal currentAverageUnitCost, decimal receivedQuantity, decimal receivedUnitCost)
    {
        if (receivedQuantity <= 0)
            throw new BusinessException("Stock quantity must be greater than zero.");
        if (receivedUnitCost < 0)
            throw new BusinessException("An inbound stock movement requires a non-negative unit cost.");

        decimal newQuantity = currentQuantity + receivedQuantity;
        decimal newAverage = newQuantity == 0
            ? 0
            : ((currentQuantity * currentAverageUnitCost) + (receivedQuantity * receivedUnitCost))
              / newQuantity;
        return (newQuantity, newAverage);
    }

    public static (decimal Quantity, decimal AverageUnitCost, decimal IssuedUnitCost) Issue(
        decimal currentQuantity, decimal currentAverageUnitCost, decimal issuedQuantity)
    {
        if (issuedQuantity <= 0)
            throw new BusinessException("Stock quantity must be greater than zero.");
        if (currentQuantity < issuedQuantity)
            throw new BusinessException(
                $"Insufficient stock. Available: {currentQuantity:0.####}, requested: {issuedQuantity:0.####}.");

        decimal newQuantity = currentQuantity - issuedQuantity;
        return (newQuantity, newQuantity == 0 ? 0 : currentAverageUnitCost, currentAverageUnitCost);
    }
}
