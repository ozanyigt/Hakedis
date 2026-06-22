using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ProjectRepository : EfRepositoryBase<Project, Guid, BaseDbContext>, IProjectRepository
{
    public ProjectRepository(BaseDbContext context) : base(context)
    {
    }
}