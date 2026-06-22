using Application.Features.ContractItems.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ContractItems;

public class ContractItemManager : IContractItemService
{
    private readonly IContractItemRepository _contractItemRepository;
    private readonly ContractItemBusinessRules _contractItemBusinessRules;

    public ContractItemManager(IContractItemRepository contractItemRepository, ContractItemBusinessRules contractItemBusinessRules)
    {
        _contractItemRepository = contractItemRepository;
        _contractItemBusinessRules = contractItemBusinessRules;
    }

    public async Task<ContractItem?> GetAsync(
        Expression<Func<ContractItem, bool>> predicate,
        Func<IQueryable<ContractItem>, IIncludableQueryable<ContractItem, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        ContractItem? contractItem = await _contractItemRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return contractItem;
    }

    public async Task<IPaginate<ContractItem>?> GetListAsync(
        Expression<Func<ContractItem, bool>>? predicate = null,
        Func<IQueryable<ContractItem>, IOrderedQueryable<ContractItem>>? orderBy = null,
        Func<IQueryable<ContractItem>, IIncludableQueryable<ContractItem, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<ContractItem> contractItemList = await _contractItemRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return contractItemList;
    }

    public async Task<ContractItem> AddAsync(ContractItem contractItem)
    {
        ContractItem addedContractItem = await _contractItemRepository.AddAsync(contractItem);

        return addedContractItem;
    }

    public async Task<ContractItem> UpdateAsync(ContractItem contractItem)
    {
        ContractItem updatedContractItem = await _contractItemRepository.UpdateAsync(contractItem);

        return updatedContractItem;
    }

    public async Task<ContractItem> DeleteAsync(ContractItem contractItem, bool permanent = false)
    {
        ContractItem deletedContractItem = await _contractItemRepository.DeleteAsync(contractItem);

        return deletedContractItem;
    }
}
