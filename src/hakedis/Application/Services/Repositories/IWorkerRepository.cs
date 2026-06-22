using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IWorkerRepository : IAsyncRepository<Worker, Guid>, IRepository<Worker, Guid>
{
}