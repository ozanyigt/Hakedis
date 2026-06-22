using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.HakedisPeriods;

public interface IHakedisPeriodService
{
    Task<HakedisPeriod?> GetAsync(
        Expression<Func<HakedisPeriod, bool>> predicate,
        Func<IQueryable<HakedisPeriod>, IIncludableQueryable<HakedisPeriod, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<HakedisPeriod>?> GetListAsync(
        Expression<Func<HakedisPeriod, bool>>? predicate = null,
        Func<IQueryable<HakedisPeriod>, IOrderedQueryable<HakedisPeriod>>? orderBy = null,
        Func<IQueryable<HakedisPeriod>, IIncludableQueryable<HakedisPeriod, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<HakedisPeriod> AddAsync(HakedisPeriod hakedisPeriod);
    Task<HakedisPeriod> UpdateAsync(HakedisPeriod hakedisPeriod);
    Task<HakedisPeriod> DeleteAsync(HakedisPeriod hakedisPeriod, bool permanent = false);
}
