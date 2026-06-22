using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Drawings;

public interface IDrawingService
{
    Task<Drawing?> GetAsync(
        Expression<Func<Drawing, bool>> predicate,
        Func<IQueryable<Drawing>, IIncludableQueryable<Drawing, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Drawing>?> GetListAsync(
        Expression<Func<Drawing, bool>>? predicate = null,
        Func<IQueryable<Drawing>, IOrderedQueryable<Drawing>>? orderBy = null,
        Func<IQueryable<Drawing>, IIncludableQueryable<Drawing, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Drawing> AddAsync(Drawing drawing);
    Task<Drawing> UpdateAsync(Drawing drawing);
    Task<Drawing> DeleteAsync(Drawing drawing, bool permanent = false);
}
