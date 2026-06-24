namespace Application.Services.HakedisPeriods;

public interface IHakedisPeriodTotalsService
{
    Task SyncDeductionTotalsAsync(Guid hakedisPeriodId, CancellationToken cancellationToken = default);
}
