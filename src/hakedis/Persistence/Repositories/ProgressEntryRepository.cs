using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ProgressEntryRepository : EfRepositoryBase<ProgressEntry, Guid, BaseDbContext>, IProgressEntryRepository
{
    public ProgressEntryRepository(BaseDbContext context) : base(context)
    {
    }
}