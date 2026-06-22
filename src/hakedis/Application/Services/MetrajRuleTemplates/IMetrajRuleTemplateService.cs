using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.MetrajRuleTemplates;

public interface IMetrajRuleTemplateService
{
    Task<MetrajRuleTemplate?> GetAsync(
        Expression<Func<MetrajRuleTemplate, bool>> predicate,
        Func<IQueryable<MetrajRuleTemplate>, IIncludableQueryable<MetrajRuleTemplate, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<MetrajRuleTemplate>?> GetListAsync(
        Expression<Func<MetrajRuleTemplate, bool>>? predicate = null,
        Func<IQueryable<MetrajRuleTemplate>, IOrderedQueryable<MetrajRuleTemplate>>? orderBy = null,
        Func<IQueryable<MetrajRuleTemplate>, IIncludableQueryable<MetrajRuleTemplate, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<MetrajRuleTemplate> AddAsync(MetrajRuleTemplate metrajRuleTemplate);
    Task<MetrajRuleTemplate> UpdateAsync(MetrajRuleTemplate metrajRuleTemplate);
    Task<MetrajRuleTemplate> DeleteAsync(MetrajRuleTemplate metrajRuleTemplate, bool permanent = false);
}
