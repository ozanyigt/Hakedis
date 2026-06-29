using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IDemoRequestRepository : IAsyncRepository<DemoRequest, Guid>, IRepository<DemoRequest, Guid>
{
}
