using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class DailySiteReportRepository(BaseDbContext context)
    : EfRepositoryBase<DailySiteReport, Guid, BaseDbContext>(context), IDailySiteReportRepository;

public class DailySiteReportPhotoRepository(BaseDbContext context)
    : EfRepositoryBase<DailySiteReportPhoto, Guid, BaseDbContext>(context), IDailySiteReportPhotoRepository;
