using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IProjectMetrajLayerMappingRepository
    : IAsyncRepository<ProjectMetrajLayerMapping, Guid>,
        IRepository<ProjectMetrajLayerMapping, Guid> { }
