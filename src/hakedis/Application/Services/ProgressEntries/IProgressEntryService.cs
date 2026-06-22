using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ProgressEntries;

public interface IProgressEntryService
{
    Task<ProgressEntry?> GetAsync(
        Expression<Func<ProgressEntry, bool>> predicate,
        Func<IQueryable<ProgressEntry>, IIncludableQueryable<ProgressEntry, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<ProgressEntry>?> GetListAsync(
        Expression<Func<ProgressEntry, bool>>? predicate = null,
        Func<IQueryable<ProgressEntry>, IOrderedQueryable<ProgressEntry>>? orderBy = null,
        Func<IQueryable<ProgressEntry>, IIncludableQueryable<ProgressEntry, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<ProgressEntry> AddAsync(ProgressEntry progressEntry);
    Task<ProgressEntry> UpdateAsync(ProgressEntry progressEntry);
    Task<ProgressEntry> DeleteAsync(ProgressEntry progressEntry, bool permanent = false);
}
