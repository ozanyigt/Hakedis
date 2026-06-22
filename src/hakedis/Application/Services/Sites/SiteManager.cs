using Application.Features.Sites.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Sites;

public class SiteManager : ISiteService
{
    private readonly ISiteRepository _siteRepository;
    private readonly SiteBusinessRules _siteBusinessRules;

    public SiteManager(ISiteRepository siteRepository, SiteBusinessRules siteBusinessRules)
    {
        _siteRepository = siteRepository;
        _siteBusinessRules = siteBusinessRules;
    }

    public async Task<Site?> GetAsync(
        Expression<Func<Site, bool>> predicate,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Site? site = await _siteRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return site;
    }

    public async Task<IPaginate<Site>?> GetListAsync(
        Expression<Func<Site, bool>>? predicate = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>>? orderBy = null,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Site> siteList = await _siteRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return siteList;
    }

    public async Task<Site> AddAsync(Site site)
    {
        Site addedSite = await _siteRepository.AddAsync(site);

        return addedSite;
    }

    public async Task<Site> UpdateAsync(Site site)
    {
        Site updatedSite = await _siteRepository.UpdateAsync(site);

        return updatedSite;
    }

    public async Task<Site> DeleteAsync(Site site, bool permanent = false)
    {
        Site deletedSite = await _siteRepository.DeleteAsync(site);

        return deletedSite;
    }
}
