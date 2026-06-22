using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class MetrajResultRepository : EfRepositoryBase<MetrajResult, Guid, BaseDbContext>, IMetrajResultRepository
{
    public MetrajResultRepository(BaseDbContext context) : base(context)
    {
    }
}