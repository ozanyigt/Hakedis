using Application.Features.Workers.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Workers;

public class WorkerManager : IWorkerService
{
    private readonly IWorkerRepository _workerRepository;
    private readonly WorkerBusinessRules _workerBusinessRules;

    public WorkerManager(IWorkerRepository workerRepository, WorkerBusinessRules workerBusinessRules)
    {
        _workerRepository = workerRepository;
        _workerBusinessRules = workerBusinessRules;
    }

    public async Task<Worker?> GetAsync(
        Expression<Func<Worker, bool>> predicate,
        Func<IQueryable<Worker>, IIncludableQueryable<Worker, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Worker? worker = await _workerRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return worker;
    }

    public async Task<IPaginate<Worker>?> GetListAsync(
        Expression<Func<Worker, bool>>? predicate = null,
        Func<IQueryable<Worker>, IOrderedQueryable<Worker>>? orderBy = null,
        Func<IQueryable<Worker>, IIncludableQueryable<Worker, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Worker> workerList = await _workerRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return workerList;
    }

    public async Task<Worker> AddAsync(Worker worker)
    {
        Worker addedWorker = await _workerRepository.AddAsync(worker);

        return addedWorker;
    }

    public async Task<Worker> UpdateAsync(Worker worker)
    {
        Worker updatedWorker = await _workerRepository.UpdateAsync(worker);

        return updatedWorker;
    }

    public async Task<Worker> DeleteAsync(Worker worker, bool permanent = false)
    {
        Worker deletedWorker = await _workerRepository.DeleteAsync(worker);

        return deletedWorker;
    }
}
