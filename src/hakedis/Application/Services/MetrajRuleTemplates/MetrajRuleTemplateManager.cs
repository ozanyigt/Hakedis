using Application.Features.MetrajRuleTemplates.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.MetrajRuleTemplates;

public class MetrajRuleTemplateManager : IMetrajRuleTemplateService
{
    private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
    private readonly MetrajRuleTemplateBusinessRules _metrajRuleTemplateBusinessRules;

    public MetrajRuleTemplateManager(IMetrajRuleTemplateRepository metrajRuleTemplateRepository, MetrajRuleTemplateBusinessRules metrajRuleTemplateBusinessRules)
    {
        _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
        _metrajRuleTemplateBusinessRules = metrajRuleTemplateBusinessRules;
    }

    public async Task<MetrajRuleTemplate?> GetAsync(
        Expression<Func<MetrajRuleTemplate, bool>> predicate,
        Func<IQueryable<MetrajRuleTemplate>, IIncludableQueryable<MetrajRuleTemplate, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        MetrajRuleTemplate? metrajRuleTemplate = await _metrajRuleTemplateRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return metrajRuleTemplate;
    }

    public async Task<IPaginate<MetrajRuleTemplate>?> GetListAsync(
        Expression<Func<MetrajRuleTemplate, bool>>? predicate = null,
        Func<IQueryable<MetrajRuleTemplate>, IOrderedQueryable<MetrajRuleTemplate>>? orderBy = null,
        Func<IQueryable<MetrajRuleTemplate>, IIncludableQueryable<MetrajRuleTemplate, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<MetrajRuleTemplate> metrajRuleTemplateList = await _metrajRuleTemplateRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return metrajRuleTemplateList;
    }

    public async Task<MetrajRuleTemplate> AddAsync(MetrajRuleTemplate metrajRuleTemplate)
    {
        MetrajRuleTemplate addedMetrajRuleTemplate = await _metrajRuleTemplateRepository.AddAsync(metrajRuleTemplate);

        return addedMetrajRuleTemplate;
    }

    public async Task<MetrajRuleTemplate> UpdateAsync(MetrajRuleTemplate metrajRuleTemplate)
    {
        MetrajRuleTemplate updatedMetrajRuleTemplate = await _metrajRuleTemplateRepository.UpdateAsync(metrajRuleTemplate);

        return updatedMetrajRuleTemplate;
    }

    public async Task<MetrajRuleTemplate> DeleteAsync(MetrajRuleTemplate metrajRuleTemplate, bool permanent = false)
    {
        MetrajRuleTemplate deletedMetrajRuleTemplate = await _metrajRuleTemplateRepository.DeleteAsync(metrajRuleTemplate);

        return deletedMetrajRuleTemplate;
    }
}
