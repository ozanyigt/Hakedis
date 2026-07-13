using Application.Features.PuantajRecords.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;
using Application.Services.CurrentUser;

namespace Application.Features.PuantajRecords.Rules;

public class PuantajRecordBusinessRules : BaseBusinessRules
{
    private readonly IPuantajRecordRepository _puantajRecordRepository;
    private readonly ILocalizationService _localizationService;
    private readonly IProjectRepository _projectRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly ICurrentUserService _currentUserService;

    public PuantajRecordBusinessRules(
        IPuantajRecordRepository puantajRecordRepository,
        ILocalizationService localizationService,
        IProjectRepository projectRepository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository,
        ICurrentUserService currentUserService)
    {
        _puantajRecordRepository = puantajRecordRepository;
        _localizationService = localizationService;
        _projectRepository = projectRepository;
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
        _currentUserService = currentUserService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, PuantajRecordsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task PuantajRecordShouldExistWhenSelected(PuantajRecord? puantajRecord)
    {
        if (puantajRecord == null)
            await throwBusinessException(PuantajRecordsBusinessMessages.PuantajRecordNotExists);
    }

    public async Task PuantajRecordIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        PuantajRecord? puantajRecord = await _puantajRecordRepository.GetAsync(
            predicate: pr => pr.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await PuantajRecordShouldExistWhenSelected(puantajRecord);
    }

    public async Task<Guid?> ResolveTenantAsync(Guid? requestedTenantId, bool required, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsGlobalAdmin)
        {
            if (required && !requestedTenantId.HasValue)
                throw new BusinessException("TenantId is required for a global administrator.");
            return requestedTenantId;
        }

        User? user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (user?.TenantId is null)
            throw new BusinessException("The current user is not assigned to a tenant.");
        if (requestedTenantId.HasValue && requestedTenantId.Value != user.TenantId.Value)
            throw new BusinessException("The requested tenant is outside the current user's tenant.");
        return user.TenantId.Value;
    }

    public async Task ValidateScopeAsync(
        Guid tenantId, Guid projectId, Guid? siteId, Guid? workerId, CancellationToken cancellationToken)
    {
        if (!await _projectRepository.AnyAsync(x => x.Id == projectId && x.TenantId == tenantId,
                cancellationToken: cancellationToken))
            throw new BusinessException("Project does not exist in the selected tenant.");
        if (siteId.HasValue && !await _siteRepository.AnyAsync(
                x => x.Id == siteId && x.ProjectId == projectId && x.TenantId == tenantId,
                cancellationToken: cancellationToken))
            throw new BusinessException("Site does not belong to the selected project and tenant.");
        if (workerId.HasValue && !await _workerRepository.AnyAsync(
                x => x.Id == workerId && x.TenantId == tenantId,
                cancellationToken: cancellationToken))
            throw new BusinessException("Worker does not belong to the selected tenant.");
    }

    public async Task<PuantajRecord> GetScopedAsync(Guid id, Guid? tenantId, CancellationToken cancellationToken)
    {
        Guid? resolvedTenantId = await ResolveTenantAsync(tenantId, false, cancellationToken);
        PuantajRecord? record = await _puantajRecordRepository.GetAsync(
            x => x.Id == id && (!resolvedTenantId.HasValue || x.TenantId == resolvedTenantId.Value),
            cancellationToken: cancellationToken);
        await PuantajRecordShouldExistWhenSelected(record);
        return record!;
    }

    public async Task EnsureUniqueWorkerDayAsync(
        Guid tenantId, Guid projectId, Guid? siteId, Guid? workerId, DateTime workDate,
        Guid? excludedId, CancellationToken cancellationToken)
    {
        if (!workerId.HasValue)
            return;

        DateTime date = workDate.Date;
        bool exists = await _puantajRecordRepository.AnyAsync(
            x => x.TenantId == tenantId
                 && x.ProjectId == projectId
                 && x.SiteId == siteId
                 && x.WorkerId == workerId
                 && x.WorkDate.Date == date
                 && (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken: cancellationToken);
        if (exists)
            throw new BusinessException("Bu işçi için aynı şantiye ve tarihte zaten bir puantaj kaydı var.");
    }
}