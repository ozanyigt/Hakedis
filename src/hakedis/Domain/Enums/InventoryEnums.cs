namespace Domain.Enums;

public enum StockMovementType
{
    Receipt = 1,
    Consumption = 2,
    TransferOut = 3,
    TransferIn = 4,
    AdjustmentIncrease = 5,
    AdjustmentDecrease = 6
}

public enum StockReferenceType
{
    Manual = 1,
    DailySiteReport = 2,
    Transfer = 3
}
