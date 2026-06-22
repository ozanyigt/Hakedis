using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IProjectRepository : IAsyncRepository<Project, Guid>, IRepository<Project, Guid>
{
}