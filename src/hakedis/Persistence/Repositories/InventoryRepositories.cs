using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class MaterialRepository(BaseDbContext context)
    : EfRepositoryBase<Material, Guid, BaseDbContext>(context), IMaterialRepository;
public class SiteStockBalanceRepository(BaseDbContext context)
    : EfRepositoryBase<SiteStockBalance, Guid, BaseDbContext>(context), ISiteStockBalanceRepository;
public class StockTransactionRepository(BaseDbContext context)
    : EfRepositoryBase<StockTransaction, Guid, BaseDbContext>(context), IStockTransactionRepository;
public class DailySiteReportMaterialLineRepository(BaseDbContext context)
    : EfRepositoryBase<DailySiteReportMaterialLine, Guid, BaseDbContext>(context), IDailySiteReportMaterialLineRepository;
public class DailySiteReportWorkforceSnapshotRepository(BaseDbContext context)
    : EfRepositoryBase<DailySiteReportWorkforceSnapshot, Guid, BaseDbContext>(context),
      IDailySiteReportWorkforceSnapshotRepository;
