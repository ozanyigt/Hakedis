using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ProjectMetrajLayerMappingRepository
    : EfRepositoryBase<ProjectMetrajLayerMapping, Guid, BaseDbContext>,
        IProjectMetrajLayerMappingRepository
{
    public ProjectMetrajLayerMappingRepository(BaseDbContext context)
        : base(context) { }
}
