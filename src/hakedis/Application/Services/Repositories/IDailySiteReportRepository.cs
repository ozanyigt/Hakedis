using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IDailySiteReportRepository : IAsyncRepository<DailySiteReport, Guid>, IRepository<DailySiteReport, Guid>
{
}

public interface IDailySiteReportPhotoRepository : IAsyncRepository<DailySiteReportPhoto, Guid>, IRepository<DailySiteReportPhoto, Guid>
{
}
