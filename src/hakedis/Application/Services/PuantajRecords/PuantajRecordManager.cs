using Application.Features.PuantajRecords.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.PuantajRecords;

public class PuantajRecordManager : IPuantajRecordService
{
    private readonly IPuantajRecordRepository _puantajRecordRepository;
    private readonly PuantajRecordBusinessRules _puantajRecordBusinessRules;

    public PuantajRecordManager(IPuantajRecordRepository puantajRecordRepository, PuantajRecordBusinessRules puantajRecordBusinessRules)
    {
        _puantajRecordRepository = puantajRecordRepository;
        _puantajRecordBusinessRules = puantajRecordBusinessRules;
    }

    public async Task<PuantajRecord?> GetAsync(
        Expression<Func<PuantajRecord, bool>> predicate,
        Func<IQueryable<PuantajRecord>, IIncludableQueryable<PuantajRecord, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return puantajRecord;
    }

    public async Task<IPaginate<PuantajRecord>?> GetListAsync(
        Expression<Func<PuantajRecord, bool>>? predicate = null,
        Func<IQueryable<PuantajRecord>, IOrderedQueryable<PuantajRecord>>? orderBy = null,
        Func<IQueryable<PuantajRecord>, IIncludableQueryable<PuantajRecord, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<PuantajRecord> puantajRecordList = await _puantajRecordRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return puantajRecordList;
    }

    public async Task<PuantajRecord> AddAsync(PuantajRecord puantajRecord)
    {
        PuantajRecord addedPuantajRecord = await _puantajRecordRepository.AddAsync(puantajRecord);

        return addedPuantajRecord;
    }

    public async Task<PuantajRecord> UpdateAsync(PuantajRecord puantajRecord)
    {
        PuantajRecord updatedPuantajRecord = await _puantajRecordRepository.UpdateAsync(puantajRecord);

        return updatedPuantajRecord;
    }

    public async Task<PuantajRecord> DeleteAsync(PuantajRecord puantajRecord, bool permanent = false)
    {
        PuantajRecord deletedPuantajRecord = await _puantajRecordRepository.DeleteAsync(puantajRecord);

        return deletedPuantajRecord;
    }
}
