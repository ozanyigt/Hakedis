using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Projects;

public interface IProjectService
{
    Task<Project?> GetAsync(
        Expression<Func<Project, bool>> predicate,
        Func<IQueryable<Project>, IIncludableQueryable<Project, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Project>?> GetListAsync(
        Expression<Func<Project, bool>>? predicate = null,
        Func<IQueryable<Project>, IOrderedQueryable<Project>>? orderBy = null,
        Func<IQueryable<Project>, IIncludableQueryable<Project, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Project> AddAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task<Project> DeleteAsync(Project project, bool permanent = false);
}
