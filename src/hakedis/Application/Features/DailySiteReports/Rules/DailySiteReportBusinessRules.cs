using Application.Services.CurrentUser;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Application.Features.DailySiteReports.Rules;

public class DailySiteReportBusinessRules(
    IDailySiteReportRepository reportRepository,
    IProjectRepository projectRepository,
    ISiteRepository siteRepository,
    ICurrentUserService currentUserService) : BaseBusinessRules
{
    public async Task<Guid?> ResolveTenantAsync(Guid? requestedTenantId, bool required, CancellationToken cancellationToken)
    {
        if (currentUserService.IsGlobalAdmin)
        {
            if (required && !requestedTenantId.HasValue)
                throw new BusinessException("TenantId is required for a global administrator.");
            return requestedTenantId;
        }

        User? user = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (user?.TenantId is null)
            throw new BusinessException("The current user is not assigned to a tenant.");
        if (requestedTenantId.HasValue && requestedTenantId.Value != user.TenantId.Value)
            throw new BusinessException("The requested tenant is outside the current user's tenant.");
        return user.TenantId.Value;
    }

    public Guid GetCurrentUserId() =>
        currentUserService.UserId ?? throw new BusinessException("An authenticated user is required.");

    public async Task ValidateProjectSiteChainAsync(Guid tenantId, Guid projectId, Guid siteId, CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetAsync(
            x => x.Id == projectId && x.TenantId == tenantId,
            enableTracking: false,
            cancellationToken: cancellationToken);
        if (project is null)
            throw new BusinessException("Project does not exist in the selected tenant.");

        Site? site = await siteRepository.GetAsync(
            x => x.Id == siteId && x.TenantId == tenantId && x.ProjectId == projectId,
            enableTracking: false,
            cancellationToken: cancellationToken);
        if (site is null)
            throw new BusinessException("Site does not belong to the selected project and tenant.");
    }

    public async Task EnsureUniqueSiteDateAsync(
        Guid tenantId, Guid siteId, DateTime reportDate, Guid? excludedId, CancellationToken cancellationToken)
    {
        DateTime date = reportDate.Date;
        bool exists = await reportRepository.AnyAsync(
            x => x.TenantId == tenantId && x.SiteId == siteId && x.ReportDate == date
                 && (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken: cancellationToken);
        if (exists)
            throw new BusinessException("Bu şantiye ve tarih için zaten aktif bir günlük saha raporu bulunuyor.");
    }

    public static void EnsureEditable(DailySiteReport report)
    {
        if (report.Status is not (DailySiteReportStatus.Draft or DailySiteReportStatus.Rejected))
            throw new BusinessException("Submitted and approved daily site reports are locked.");
    }

    public static void EnsureTransition(DailySiteReport report, DailySiteReportStatus target)
    {
        bool valid = target switch
        {
            DailySiteReportStatus.Submitted =>
                report.Status is DailySiteReportStatus.Draft or DailySiteReportStatus.Rejected,
            DailySiteReportStatus.Approved or DailySiteReportStatus.Rejected =>
                report.Status == DailySiteReportStatus.Submitted,
            _ => false
        };
        if (!valid)
            throw new BusinessException($"Daily site report cannot transition from {report.Status} to {target}.");
    }

    public static void ValidateTemperatures(decimal? min, decimal? max)
    {
        if (min.HasValue && max.HasValue && min.Value > max.Value)
            throw new BusinessException("Minimum temperature cannot exceed maximum temperature.");
    }

    public static void EnsurePhotoCanBeAdded(int existingPhotoCount, long sizeBytes, string fileName, string contentType)
    {
        if (existingPhotoCount >= 6)
            throw new BusinessException("A daily site report can contain at most 6 photos.");
        if (sizeBytes <= 0)
            throw new BusinessException("A photo file is required.");
        if (sizeBytes > 8 * 1024 * 1024)
            throw new BusinessException("Photo size cannot exceed 8 MB.");

        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        string[] extensions = [".jpg", ".jpeg", ".png", ".webp"];
        string[] contentTypes = ["image/jpeg", "image/png", "image/webp"];
        if (!extensions.Contains(extension) || !contentTypes.Contains(contentType.ToLowerInvariant()))
            throw new BusinessException("Only jpg, jpeg, png and webp photos are supported.");
    }
}
