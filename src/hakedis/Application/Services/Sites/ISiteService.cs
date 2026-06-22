using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Sites;

public interface ISiteService
{
    Task<Site?> GetAsync(
        Expression<Func<Site, bool>> predicate,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Site>?> GetListAsync(
        Expression<Func<Site, bool>>? predicate = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>>? orderBy = null,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Site> AddAsync(Site site);
    Task<Site> UpdateAsync(Site site);
    Task<Site> DeleteAsync(Site site, bool permanent = false);
}
