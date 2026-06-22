using Application.Features.HakedisPeriods.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.HakedisPeriods;

public class HakedisPeriodManager : IHakedisPeriodService
{
    private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
    private readonly HakedisPeriodBusinessRules _hakedisPeriodBusinessRules;

    public HakedisPeriodManager(IHakedisPeriodRepository hakedisPeriodRepository, HakedisPeriodBusinessRules hakedisPeriodBusinessRules)
    {
        _hakedisPeriodRepository = hakedisPeriodRepository;
        _hakedisPeriodBusinessRules = hakedisPeriodBusinessRules;
    }

    public async Task<HakedisPeriod?> GetAsync(
        Expression<Func<HakedisPeriod, bool>> predicate,
        Func<IQueryable<HakedisPeriod>, IIncludableQueryable<HakedisPeriod, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        HakedisPeriod? hakedisPeriod = await _hakedisPeriodRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return hakedisPeriod;
    }

    public async Task<IPaginate<HakedisPeriod>?> GetListAsync(
        Expression<Func<HakedisPeriod, bool>>? predicate = null,
        Func<IQueryable<HakedisPeriod>, IOrderedQueryable<HakedisPeriod>>? orderBy = null,
        Func<IQueryable<HakedisPeriod>, IIncludableQueryable<HakedisPeriod, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<HakedisPeriod> hakedisPeriodList = await _hakedisPeriodRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return hakedisPeriodList;
    }

    public async Task<HakedisPeriod> AddAsync(HakedisPeriod hakedisPeriod)
    {
        HakedisPeriod addedHakedisPeriod = await _hakedisPeriodRepository.AddAsync(hakedisPeriod);

        return addedHakedisPeriod;
    }

    public async Task<HakedisPeriod> UpdateAsync(HakedisPeriod hakedisPeriod)
    {
        HakedisPeriod updatedHakedisPeriod = await _hakedisPeriodRepository.UpdateAsync(hakedisPeriod);

        return updatedHakedisPeriod;
    }

    public async Task<HakedisPeriod> DeleteAsync(HakedisPeriod hakedisPeriod, bool permanent = false)
    {
        HakedisPeriod deletedHakedisPeriod = await _hakedisPeriodRepository.DeleteAsync(hakedisPeriod);

        return deletedHakedisPeriod;
    }
}
