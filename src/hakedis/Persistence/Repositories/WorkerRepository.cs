using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class WorkerRepository : EfRepositoryBase<Worker, Guid, BaseDbContext>, IWorkerRepository
{
    public WorkerRepository(BaseDbContext context) : base(context)
    {
    }
}