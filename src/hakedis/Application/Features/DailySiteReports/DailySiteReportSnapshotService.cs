using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.DailySiteReports;

public class DailySiteReportSnapshotService(
    IPuantajRecordRepository puantajRepository,
    IDailySiteReportWorkforceSnapshotRepository snapshotRepository)
{
    public async Task<DailySiteReportWorkforceDto> PreviewAsync(
        Guid tenantId, Guid projectId, Guid siteId, DateTime reportDate, CancellationToken cancellationToken)
    {
        DateTime date = reportDate.Date;
        IPaginate<PuantajRecord> records = await puantajRepository.GetListAsync(
            x => x.TenantId == tenantId && x.ProjectId == projectId && x.SiteId == siteId
                 && x.WorkDate.Date == date && x.Status != PuantajStatus.Rejected,
            include: q => q.Include(x => x.Worker),
            size: 10000, enableTracking: false, cancellationToken: cancellationToken);
        IPaginate<PuantajRecord> siteLessRecords = await puantajRepository.GetListAsync(
            x => x.TenantId == tenantId && x.ProjectId == projectId && x.SiteId == null
                 && x.WorkDate.Date == date && x.Status != PuantajStatus.Rejected,
            size: 1, enableTracking: false, cancellationToken: cancellationToken);
        List<DailySiteReportWorkforceSnapshotDto> rows = records.Items
            .Select(x => new DailySiteReportWorkforceSnapshotDto
            {
                SourcePuantajRecordId = x.Id,
                WorkerId = x.WorkerId,
                WorkerName = x.Worker?.FullName ?? "Atanmamış işçi",
                Trade = x.Worker?.Trade,
                WorkType = x.WorkType,
                DayCount = x.DayCount,
                OvertimeHours = x.OvertimeHours,
                PuantajStatusAtCapture = x.Status
            })
            .OrderBy(x => x.WorkerName)
            .ThenBy(x => x.WorkType)
            .ToList();
        return new DailySiteReportWorkforceDto
        {
            Rows = rows,
            SiteLessCount = siteLessRecords.Count
        };
    }

    public async Task CaptureAsync(DailySiteReport report, CancellationToken cancellationToken)
    {
        IPaginate<DailySiteReportWorkforceSnapshot> existing = await snapshotRepository.GetListAsync(
            x => x.DailySiteReportId == report.Id, size: 10000, cancellationToken: cancellationToken);
        foreach (DailySiteReportWorkforceSnapshot item in existing.Items)
            await snapshotRepository.DeleteAsync(item, permanent: true);

        DailySiteReportWorkforceDto preview = await PreviewAsync(
            report.TenantId, report.ProjectId, report.SiteId, report.ReportDate, cancellationToken);
        Guid captureBatchId = Guid.NewGuid();
        DateTime capturedAt = DateTime.UtcNow;
        foreach (DailySiteReportWorkforceSnapshotDto item in preview.Rows)
        {
            await snapshotRepository.AddAsync(new DailySiteReportWorkforceSnapshot
            {
                DailySiteReportId = report.Id,
                SourcePuantajRecordId = item.SourcePuantajRecordId!.Value,
                WorkerId = item.WorkerId,
                WorkerName = item.WorkerName,
                Trade = item.Trade,
                WorkType = item.WorkType,
                DayCount = item.DayCount,
                OvertimeHours = item.OvertimeHours,
                PuantajStatusAtCapture = item.PuantajStatusAtCapture!.Value,
                CaptureBatchId = captureBatchId,
                CapturedAt = capturedAt
            });
        }
    }
}
