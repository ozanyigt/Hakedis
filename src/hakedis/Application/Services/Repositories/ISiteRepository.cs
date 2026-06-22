using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ISiteRepository : IAsyncRepository<Site, Guid>, IRepository<Site, Guid>
{
}