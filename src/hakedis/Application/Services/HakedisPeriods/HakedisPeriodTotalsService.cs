using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.HakedisPeriods;

public class HakedisPeriodTotalsService : IHakedisPeriodTotalsService
{
    private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
    private readonly IHakedisDeductionLineRepository _hakedisDeductionLineRepository;

    public HakedisPeriodTotalsService(
        IHakedisPeriodRepository hakedisPeriodRepository,
        IHakedisDeductionLineRepository hakedisDeductionLineRepository
    )
    {
        _hakedisPeriodRepository = hakedisPeriodRepository;
        _hakedisDeductionLineRepository = hakedisDeductionLineRepository;
    }

    public async Task SyncDeductionTotalsAsync(Guid hakedisPeriodId, CancellationToken cancellationToken = default)
    {
        HakedisPeriod? period = await _hakedisPeriodRepository.GetAsync(
            predicate: p => p.Id == hakedisPeriodId,
            cancellationToken: cancellationToken
        );

        if (period is null)
            return;

        IPaginate<HakedisDeductionLine> lines = await _hakedisDeductionLineRepository.GetListAsync(
            predicate: line => line.HakedisPeriodId == hakedisPeriodId,
            index: 0,
            size: 1000,
            cancellationToken: cancellationToken
        );

        decimal deductionTotal = lines.Items.Sum(line => line.Amount);
        period.DeductionAmount = deductionTotal;
        period.NetAmount = period.TotalAmount - deductionTotal;

        await _hakedisPeriodRepository.UpdateAsync(period);
    }
}
