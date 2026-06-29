using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class DemoRequestRepository : EfRepositoryBase<DemoRequest, Guid, BaseDbContext>, IDemoRequestRepository
{
    public DemoRequestRepository(BaseDbContext context)
        : base(context) { }
}
