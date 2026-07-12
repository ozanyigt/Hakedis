using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IMetrajPolicyRepository : IAsyncRepository<MetrajPolicy, Guid>, IRepository<MetrajPolicy, Guid>
{
}
