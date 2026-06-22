using Application.Features.ProgressEntries.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ProgressEntries;

public class ProgressEntryManager : IProgressEntryService
{
    private readonly IProgressEntryRepository _progressEntryRepository;
    private readonly ProgressEntryBusinessRules _progressEntryBusinessRules;

    public ProgressEntryManager(IProgressEntryRepository progressEntryRepository, ProgressEntryBusinessRules progressEntryBusinessRules)
    {
        _progressEntryRepository = progressEntryRepository;
        _progressEntryBusinessRules = progressEntryBusinessRules;
    }

    public async Task<ProgressEntry?> GetAsync(
        Expression<Func<ProgressEntry, bool>> predicate,
        Func<IQueryable<ProgressEntry>, IIncludableQueryable<ProgressEntry, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        ProgressEntry? progressEntry = await _progressEntryRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return progressEntry;
    }

    public async Task<IPaginate<ProgressEntry>?> GetListAsync(
        Expression<Func<ProgressEntry, bool>>? predicate = null,
        Func<IQueryable<ProgressEntry>, IOrderedQueryable<ProgressEntry>>? orderBy = null,
        Func<IQueryable<ProgressEntry>, IIncludableQueryable<ProgressEntry, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<ProgressEntry> progressEntryList = await _progressEntryRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return progressEntryList;
    }

    public async Task<ProgressEntry> AddAsync(ProgressEntry progressEntry)
    {
        ProgressEntry addedProgressEntry = await _progressEntryRepository.AddAsync(progressEntry);

        return addedProgressEntry;
    }

    public async Task<ProgressEntry> UpdateAsync(ProgressEntry progressEntry)
    {
        ProgressEntry updatedProgressEntry = await _progressEntryRepository.UpdateAsync(progressEntry);

        return updatedProgressEntry;
    }

    public async Task<ProgressEntry> DeleteAsync(ProgressEntry progressEntry, bool permanent = false)
    {
        ProgressEntry deletedProgressEntry = await _progressEntryRepository.DeleteAsync(progressEntry);

        return deletedProgressEntry;
    }
}
