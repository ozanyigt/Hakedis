using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ContractItems;

public interface IContractItemService
{
    Task<ContractItem?> GetAsync(
        Expression<Func<ContractItem, bool>> predicate,
        Func<IQueryable<ContractItem>, IIncludableQueryable<ContractItem, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<ContractItem>?> GetListAsync(
        Expression<Func<ContractItem, bool>>? predicate = null,
        Func<IQueryable<ContractItem>, IOrderedQueryable<ContractItem>>? orderBy = null,
        Func<IQueryable<ContractItem>, IIncludableQueryable<ContractItem, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<ContractItem> AddAsync(ContractItem contractItem);
    Task<ContractItem> UpdateAsync(ContractItem contractItem);
    Task<ContractItem> DeleteAsync(ContractItem contractItem, bool permanent = false);
}
