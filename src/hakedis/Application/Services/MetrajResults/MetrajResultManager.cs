using Application.Features.MetrajResults.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.MetrajResults;

public class MetrajResultManager : IMetrajResultService
{
    private readonly IMetrajResultRepository _metrajResultRepository;
    private readonly MetrajResultBusinessRules _metrajResultBusinessRules;

    public MetrajResultManager(IMetrajResultRepository metrajResultRepository, MetrajResultBusinessRules metrajResultBusinessRules)
    {
        _metrajResultRepository = metrajResultRepository;
        _metrajResultBusinessRules = metrajResultBusinessRules;
    }

    public async Task<MetrajResult?> GetAsync(
        Expression<Func<MetrajResult, bool>> predicate,
        Func<IQueryable<MetrajResult>, IIncludableQueryable<MetrajResult, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        MetrajResult? metrajResult = await _metrajResultRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return metrajResult;
    }

    public async Task<IPaginate<MetrajResult>?> GetListAsync(
        Expression<Func<MetrajResult, bool>>? predicate = null,
        Func<IQueryable<MetrajResult>, IOrderedQueryable<MetrajResult>>? orderBy = null,
        Func<IQueryable<MetrajResult>, IIncludableQueryable<MetrajResult, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<MetrajResult> metrajResultList = await _metrajResultRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return metrajResultList;
    }

    public async Task<MetrajResult> AddAsync(MetrajResult metrajResult)
    {
        MetrajResult addedMetrajResult = await _metrajResultRepository.AddAsync(metrajResult);

        return addedMetrajResult;
    }

    public async Task<MetrajResult> UpdateAsync(MetrajResult metrajResult)
    {
        MetrajResult updatedMetrajResult = await _metrajResultRepository.UpdateAsync(metrajResult);

        return updatedMetrajResult;
    }

    public async Task<MetrajResult> DeleteAsync(MetrajResult metrajResult, bool permanent = false)
    {
        MetrajResult deletedMetrajResult = await _metrajResultRepository.DeleteAsync(metrajResult);

        return deletedMetrajResult;
    }
}
