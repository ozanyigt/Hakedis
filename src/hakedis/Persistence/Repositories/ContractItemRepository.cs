using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ContractItemRepository : EfRepositoryBase<ContractItem, Guid, BaseDbContext>, IContractItemRepository
{
    public ContractItemRepository(BaseDbContext context) : base(context)
    {
    }
}