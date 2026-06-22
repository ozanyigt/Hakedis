using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class PuantajRecordRepository : EfRepositoryBase<PuantajRecord, Guid, BaseDbContext>, IPuantajRecordRepository
{
    public PuantajRecordRepository(BaseDbContext context) : base(context)
    {
    }
}