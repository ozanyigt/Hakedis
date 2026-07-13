using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IMaterialRepository : IAsyncRepository<Material, Guid>, IRepository<Material, Guid>;
public interface ISiteStockBalanceRepository : IAsyncRepository<SiteStockBalance, Guid>, IRepository<SiteStockBalance, Guid>;
public interface IStockTransactionRepository : IAsyncRepository<StockTransaction, Guid>, IRepository<StockTransaction, Guid>;
public interface IDailySiteReportMaterialLineRepository :
    IAsyncRepository<DailySiteReportMaterialLine, Guid>, IRepository<DailySiteReportMaterialLine, Guid>;
public interface IDailySiteReportWorkforceSnapshotRepository :
    IAsyncRepository<DailySiteReportWorkforceSnapshot, Guid>, IRepository<DailySiteReportWorkforceSnapshot, Guid>;
