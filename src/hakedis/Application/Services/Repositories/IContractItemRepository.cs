using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IContractItemRepository : IAsyncRepository<ContractItem, Guid>, IRepository<ContractItem, Guid>
{
}