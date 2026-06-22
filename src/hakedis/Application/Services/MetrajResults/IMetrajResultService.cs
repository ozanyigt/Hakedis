using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.MetrajResults;

public interface IMetrajResultService
{
    Task<MetrajResult?> GetAsync(
        Expression<Func<MetrajResult, bool>> predicate,
        Func<IQueryable<MetrajResult>, IIncludableQueryable<MetrajResult, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<MetrajResult>?> GetListAsync(
        Expression<Func<MetrajResult, bool>>? predicate = null,
        Func<IQueryable<MetrajResult>, IOrderedQueryable<MetrajResult>>? orderBy = null,
        Func<IQueryable<MetrajResult>, IIncludableQueryable<MetrajResult, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<MetrajResult> AddAsync(MetrajResult metrajResult);
    Task<MetrajResult> UpdateAsync(MetrajResult metrajResult);
    Task<MetrajResult> DeleteAsync(MetrajResult metrajResult, bool permanent = false);
}
