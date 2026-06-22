using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class SiteRepository : EfRepositoryBase<Site, Guid, BaseDbContext>, ISiteRepository
{
    public SiteRepository(BaseDbContext context) : base(context)
    {
    }
}