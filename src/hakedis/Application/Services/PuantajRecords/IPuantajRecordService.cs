using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.PuantajRecords;

public interface IPuantajRecordService
{
    Task<PuantajRecord?> GetAsync(
        Expression<Func<PuantajRecord, bool>> predicate,
        Func<IQueryable<PuantajRecord>, IIncludableQueryable<PuantajRecord, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<PuantajRecord>?> GetListAsync(
        Expression<Func<PuantajRecord, bool>>? predicate = null,
        Func<IQueryable<PuantajRecord>, IOrderedQueryable<PuantajRecord>>? orderBy = null,
        Func<IQueryable<PuantajRecord>, IIncludableQueryable<PuantajRecord, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<PuantajRecord> AddAsync(PuantajRecord puantajRecord);
    Task<PuantajRecord> UpdateAsync(PuantajRecord puantajRecord);
    Task<PuantajRecord> DeleteAsync(PuantajRecord puantajRecord, bool permanent = false);
}
