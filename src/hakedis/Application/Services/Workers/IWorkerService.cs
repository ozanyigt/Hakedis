using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Workers;

public interface IWorkerService
{
    Task<Worker?> GetAsync(
        Expression<Func<Worker, bool>> predicate,
        Func<IQueryable<Worker>, IIncludableQueryable<Worker, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Worker>?> GetListAsync(
        Expression<Func<Worker, bool>>? predicate = null,
        Func<IQueryable<Worker>, IOrderedQueryable<Worker>>? orderBy = null,
        Func<IQueryable<Worker>, IIncludableQueryable<Worker, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Worker> AddAsync(Worker worker);
    Task<Worker> UpdateAsync(Worker worker);
    Task<Worker> DeleteAsync(Worker worker, bool permanent = false);
}
