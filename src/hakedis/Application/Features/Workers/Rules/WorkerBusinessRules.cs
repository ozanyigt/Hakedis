using Application.Features.Workers.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Workers.Rules;

public class WorkerBusinessRules : BaseBusinessRules
{
    private readonly IWorkerRepository _workerRepository;
    private readonly ILocalizationService _localizationService;

    public WorkerBusinessRules(IWorkerRepository workerRepository, ILocalizationService localizationService)
    {
        _workerRepository = workerRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, WorkersBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task WorkerShouldExistWhenSelected(Worker? worker)
    {
        if (worker == null)
            await throwBusinessException(WorkersBusinessMessages.WorkerNotExists);
    }

    public async Task WorkerIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Worker? worker = await _workerRepository.GetAsync(
            predicate: w => w.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await WorkerShouldExistWhenSelected(worker);
    }
}