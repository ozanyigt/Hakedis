using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class HakedisPeriodRepository : EfRepositoryBase<HakedisPeriod, Guid, BaseDbContext>, IHakedisPeriodRepository
{
    public HakedisPeriodRepository(BaseDbContext context) : base(context)
    {
    }
}