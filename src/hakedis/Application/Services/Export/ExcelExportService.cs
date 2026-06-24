using Application.Services.Repositories;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.Export;

public class ExcelExportService : IExcelExportService
{
    private const int ExportPageSize = 10_000;

    private readonly IProjectRepository _projectRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPuantajRecordRepository _puantajRecordRepository;
    private readonly IMetrajResultRepository _metrajResultRepository;
    private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
    private readonly IProgressEntryRepository _progressEntryRepository;
    private readonly IContractItemRepository _contractItemRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IDrawingRepository _drawingRepository;
    private readonly IHakedisDeductionLineRepository _hakedisDeductionLineRepository;

    public ExcelExportService(
        IProjectRepository projectRepository,
        ITenantRepository tenantRepository,
        IPuantajRecordRepository puantajRecordRepository,
        IMetrajResultRepository metrajResultRepository,
        IHakedisPeriodRepository hakedisPeriodRepository,
        IProgressEntryRepository progressEntryRepository,
        IContractItemRepository contractItemRepository,
        ISiteRepository siteRepository,
        IDrawingRepository drawingRepository,
        IHakedisDeductionLineRepository hakedisDeductionLineRepository
    )
    {
        _projectRepository = projectRepository;
        _tenantRepository = tenantRepository;
        _puantajRecordRepository = puantajRecordRepository;
        _metrajResultRepository = metrajResultRepository;
        _hakedisPeriodRepository = hakedisPeriodRepository;
        _progressEntryRepository = progressEntryRepository;
        _contractItemRepository = contractItemRepository;
        _siteRepository = siteRepository;
        _drawingRepository = drawingRepository;
        _hakedisDeductionLineRepository = hakedisDeductionLineRepository;
    }

    public async Task<ExportFileResponse> ExportProjectsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        IPaginate<Project> projects = await _projectRepository.GetListAsync(
            predicate: p => p.TenantId == tenantId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Projeler");
        WriteHeader(
            sheet,
            "Kod",
            "Proje Adı",
            "Konum",
            "İşveren",
            "Sözleşme Tutarı",
            "Başlangıç",
            "Bitiş",
            "Durum",
            "Açıklama"
        );

        var row = 2;
        foreach (Project project in projects.Items.OrderBy(p => p.Name))
        {
            sheet.Cell(row, 1).Value = project.Code ?? string.Empty;
            sheet.Cell(row, 2).Value = project.Name;
            sheet.Cell(row, 3).Value = project.Location ?? string.Empty;
            sheet.Cell(row, 4).Value = project.ClientName ?? string.Empty;
            sheet.Cell(row, 5).Value = project.ContractAmount;
            sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 6).Value = project.StartDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            sheet.Cell(row, 7).Value = project.EndDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            sheet.Cell(row, 8).Value = ExportEnumLabels.GetProjectStatusLabel(project.Status);
            sheet.Cell(row, 9).Value = project.Description ?? string.Empty;
            row++;
        }

        FormatSheet(sheet);
        return ToResponse(workbook, $"projeler_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    public async Task<ExportFileResponse> ExportPuantajAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        Project project = await GetProjectAsync(tenantId, projectId, cancellationToken);
        Tenant? tenant = await _tenantRepository.GetAsync(
            predicate: t => t.Id == tenantId,
            cancellationToken: cancellationToken
        );

        IPaginate<PuantajRecord> records = await _puantajRecordRepository.GetListAsync(
            predicate: r => r.TenantId == tenantId && r.ProjectId == projectId,
            include: query => query.Include(r => r.Worker).Include(r => r.Site),
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Puantaj");
        int headerRow = WritePuantajTitleSection(sheet, tenant?.Name ?? "—", project.Name);
        WriteHeader(
            sheet,
            headerRow,
            "Firma",
            "Proje",
            "Tarih",
            "İşçi",
            "Şantiye",
            "İş Tipi",
            "Gün",
            "Mesai (saat)",
            "Durum",
            "Not"
        );

        string tenantName = tenant?.Name ?? "—";
        var row = headerRow + 1;
        foreach (PuantajRecord record in records.Items.OrderByDescending(r => r.WorkDate))
        {
            sheet.Cell(row, 1).Value = tenantName;
            sheet.Cell(row, 2).Value = project.Name;
            sheet.Cell(row, 3).Value = record.WorkDate.ToString("dd.MM.yyyy");
            sheet.Cell(row, 4).Value = record.Worker?.FullName ?? string.Empty;
            sheet.Cell(row, 5).Value = record.Site?.Name ?? string.Empty;
            sheet.Cell(row, 6).Value = ExportEnumLabels.GetWorkTypeLabel(record.WorkType);
            sheet.Cell(row, 7).Value = record.DayCount;
            sheet.Cell(row, 8).Value = record.OvertimeHours;
            sheet.Cell(row, 9).Value = ExportEnumLabels.GetPuantajStatusLabel(record.Status);
            sheet.Cell(row, 10).Value = record.Notes ?? string.Empty;
            row++;
        }

        FormatSheet(sheet, headerRow);
        return ToResponse(workbook, BuildProjectFileName("puantaj", project));
    }

    public async Task<ExportFileResponse> ExportMetrajAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        Project project = await GetProjectAsync(tenantId, projectId, cancellationToken);

        IPaginate<MetrajResult> results = await _metrajResultRepository.GetListAsync(
            predicate: r => r.TenantId == tenantId && r.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        Dictionary<Guid, string> sites = await LoadSiteNamesAsync(tenantId, projectId, cancellationToken);
        Dictionary<Guid, string> drawings = await LoadDrawingNamesAsync(tenantId, projectId, cancellationToken);

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Metraj");
        WriteHeader(
            sheet,
            "Kalem",
            "Birim",
            "Miktar",
            "Kat",
            "Mahal",
            "Çizim",
            "Şantiye",
            "Hesap Tarihi",
            "Not"
        );

        var row = 2;
        foreach (MetrajResult result in results.Items.OrderBy(r => r.KalemType).ThenBy(r => r.FloorName))
        {
            sheet.Cell(row, 1).Value = ExportEnumLabels.GetMetrajKalemLabel(result.KalemType);
            sheet.Cell(row, 2).Value = ExportEnumLabels.GetMeasurementUnitLabel(result.Unit);
            sheet.Cell(row, 3).Value = result.Quantity;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.###";
            sheet.Cell(row, 4).Value = result.FloorName ?? string.Empty;
            sheet.Cell(row, 5).Value = result.SpaceName ?? string.Empty;
            sheet.Cell(row, 6).Value = drawings.TryGetValue(result.DrawingId, out string? drawingName) ? drawingName : string.Empty;
            sheet.Cell(row, 7).Value = result.SiteId.HasValue && sites.TryGetValue(result.SiteId.Value, out string? siteName)
                ? siteName
                : string.Empty;
            sheet.Cell(row, 8).Value = result.CalculatedAt.ToString("dd.MM.yyyy HH:mm");
            sheet.Cell(row, 9).Value = result.Notes ?? string.Empty;
            row++;
        }

        FormatSheet(sheet);
        return ToResponse(workbook, BuildProjectFileName("metraj", project));
    }

    public async Task<ExportFileResponse> ExportContractItemsAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        Project project = await GetProjectAsync(tenantId, projectId, cancellationToken);

        IPaginate<ContractItem> items = await _contractItemRepository.GetListAsync(
            predicate: c => c.TenantId == tenantId && c.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Sözleşme Kalemleri");
        WriteHeader(
            sheet,
            "Sıra",
            "Kalem",
            "Açıklama",
            "Birim",
            "Birim Fiyat",
            "Sözleşme Miktarı",
            "Tutar"
        );

        var row = 2;
        foreach (ContractItem item in items.Items.OrderBy(c => c.SortOrder))
        {
            decimal amount = item.UnitPrice * (item.ContractQuantity ?? 0);
            sheet.Cell(row, 1).Value = item.SortOrder;
            sheet.Cell(row, 2).Value = ExportEnumLabels.GetMetrajKalemLabel(item.KalemType);
            sheet.Cell(row, 3).Value = item.Description;
            sheet.Cell(row, 4).Value = ExportEnumLabels.GetMeasurementUnitLabel(item.Unit);
            sheet.Cell(row, 5).Value = item.UnitPrice;
            sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 6).Value = item.ContractQuantity ?? 0;
            sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.###";
            sheet.Cell(row, 7).Value = amount;
            sheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }

        FormatSheet(sheet);
        return ToResponse(workbook, BuildProjectFileName("sozlesme-kalemleri", project));
    }

    public async Task<ExportFileResponse> ExportHakedisAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        Project project = await GetProjectAsync(tenantId, projectId, cancellationToken);

        IPaginate<HakedisPeriod> periods = await _hakedisPeriodRepository.GetListAsync(
            predicate: p => p.TenantId == tenantId && p.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        IPaginate<HakedisDeductionLine> deductionLines = await _hakedisDeductionLineRepository.GetListAsync(
            predicate: line => line.TenantId == tenantId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        IPaginate<ProgressEntry> progressEntries = await _progressEntryRepository.GetListAsync(
            predicate: p => p.TenantId == tenantId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        IPaginate<ContractItem> contractItems = await _contractItemRepository.GetListAsync(
            predicate: c => c.TenantId == tenantId && c.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        var periodIds = periods.Items.Select(p => p.Id).ToHashSet();
        var periodById = periods.Items.ToDictionary(p => p.Id);
        var contractById = contractItems.Items.ToDictionary(c => c.Id);

        using var workbook = new XLWorkbook();
        WritePeriodsSheet(workbook, periods.Items);
        WriteProgressSheet(
            workbook,
            progressEntries.Items.Where(p => periodIds.Contains(p.HakedisPeriodId)),
            periodById,
            contractById
        );
        WriteDeductionLinesSheet(
            workbook,
            deductionLines.Items.Where(line => periodIds.Contains(line.HakedisPeriodId)),
            periodById
        );
        WriteContractItemsSheet(workbook, contractItems.Items);

        return ToResponse(workbook, BuildProjectFileName("hakedis", project));
    }

    private static void WritePeriodsSheet(XLWorkbook workbook, IEnumerable<HakedisPeriod> periods)
    {
        var sheet = workbook.Worksheets.Add("Dönemler");
        WriteHeader(
            sheet,
            "Dönem No",
            "Ad",
            "Başlangıç",
            "Bitiş",
            "Durum",
            "Brüt Tutar",
            "Kesinti",
            "Net Tutar",
            "Not"
        );

        var row = 2;
        foreach (HakedisPeriod period in periods.OrderBy(p => p.PeriodNumber))
        {
            sheet.Cell(row, 1).Value = period.PeriodNumber;
            sheet.Cell(row, 2).Value = period.Name;
            sheet.Cell(row, 3).Value = period.PeriodStart.ToString("dd.MM.yyyy");
            sheet.Cell(row, 4).Value = period.PeriodEnd.ToString("dd.MM.yyyy");
            sheet.Cell(row, 5).Value = ExportEnumLabels.GetHakedisStatusLabel(period.Status);
            sheet.Cell(row, 6).Value = period.TotalAmount;
            sheet.Cell(row, 7).Value = period.DeductionAmount;
            sheet.Cell(row, 8).Value = period.NetAmount;
            sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 9).Value = period.Notes ?? string.Empty;
            row++;
        }

        FormatSheet(sheet);
    }

    private static void WriteProgressSheet(
        XLWorkbook workbook,
        IEnumerable<ProgressEntry> entries,
        IReadOnlyDictionary<Guid, HakedisPeriod> periodById,
        IReadOnlyDictionary<Guid, ContractItem> contractById
    )
    {
        var sheet = workbook.Worksheets.Add("İlerleme");
        WriteHeader(
            sheet,
            "Dönem",
            "Kalem",
            "Açıklama",
            "Dönem Miktarı",
            "Kümülatif Miktar",
            "Dönem Tutarı",
            "Manuel",
            "Not"
        );

        var row = 2;
        foreach (ProgressEntry entry in entries.OrderBy(e => e.HakedisPeriodId))
        {
            periodById.TryGetValue(entry.HakedisPeriodId, out HakedisPeriod? period);
            contractById.TryGetValue(entry.ContractItemId, out ContractItem? contractItem);

            sheet.Cell(row, 1).Value = period != null ? $"{period.PeriodNumber} — {period.Name}" : string.Empty;
            sheet.Cell(row, 2).Value = contractItem != null ? ExportEnumLabels.GetMetrajKalemLabel(contractItem.KalemType) : string.Empty;
            sheet.Cell(row, 3).Value = contractItem?.Description ?? string.Empty;
            sheet.Cell(row, 4).Value = entry.QuantityThisPeriod;
            sheet.Cell(row, 5).Value = entry.CumulativeQuantity;
            sheet.Cell(row, 6).Value = entry.AmountThisPeriod;
            sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.###";
            sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.###";
            sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 7).Value = entry.IsManualEntry ? "Evet" : "Hayır";
            sheet.Cell(row, 8).Value = entry.Notes ?? string.Empty;
            row++;
        }

        FormatSheet(sheet);
    }

    private static void WriteDeductionLinesSheet(
        XLWorkbook workbook,
        IEnumerable<HakedisDeductionLine> lines,
        IReadOnlyDictionary<Guid, HakedisPeriod> periodById
    )
    {
        var sheet = workbook.Worksheets.Add("Kesintiler");
        WriteHeader(sheet, "Dönem", "Kategori", "Açıklama", "Tutar", "Not");

        var row = 2;
        foreach (HakedisDeductionLine line in lines.OrderBy(l => l.HakedisPeriodId))
        {
            periodById.TryGetValue(line.HakedisPeriodId, out HakedisPeriod? period);
            sheet.Cell(row, 1).Value = period != null ? $"{period.PeriodNumber} — {period.Name}" : string.Empty;
            sheet.Cell(row, 2).Value = GetDeductionCategoryLabel(line.Category);
            sheet.Cell(row, 3).Value = line.Description;
            sheet.Cell(row, 4).Value = line.Amount;
            sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 5).Value = line.Notes ?? string.Empty;
            row++;
        }

        FormatSheet(sheet);
    }

    private static string GetDeductionCategoryLabel(DeductionCategory category) =>
        category switch
        {
            DeductionCategory.Malzeme => "Malzeme",
            DeductionCategory.Makine => "Makine",
            DeductionCategory.Yemek => "Yemek",
            DeductionCategory.Ilave => "İlave",
            DeductionCategory.Diger => "Diğer",
            _ => category.ToString()
        };

    private static void WriteContractItemsSheet(XLWorkbook workbook, IList<ContractItem> items)
    {
        var sheet = workbook.Worksheets.Add("Sözleşme Kalemleri");
        WriteHeader(sheet, "Sıra", "Kalem", "Açıklama", "Birim", "Birim Fiyat", "Sözleşme Miktarı");

        var row = 2;
        foreach (ContractItem item in items.OrderBy(c => c.SortOrder))
        {
            sheet.Cell(row, 1).Value = item.SortOrder;
            sheet.Cell(row, 2).Value = ExportEnumLabels.GetMetrajKalemLabel(item.KalemType);
            sheet.Cell(row, 3).Value = item.Description;
            sheet.Cell(row, 4).Value = ExportEnumLabels.GetMeasurementUnitLabel(item.Unit);
            sheet.Cell(row, 5).Value = item.UnitPrice;
            sheet.Cell(row, 6).Value = item.ContractQuantity ?? 0;
            sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
            sheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.###";
            row++;
        }

        FormatSheet(sheet);
    }

    private async Task<Project> GetProjectAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetAsync(
            predicate: p => p.Id == projectId && p.TenantId == tenantId,
            cancellationToken: cancellationToken
        );

        if (project is null)
            throw new InvalidOperationException("Proje bulunamadı.");

        return project;
    }

    private async Task<Dictionary<Guid, string>> LoadSiteNamesAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        IPaginate<Site> sites = await _siteRepository.GetListAsync(
            predicate: s => s.TenantId == tenantId && s.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        return sites.Items.ToDictionary(s => s.Id, s => s.Name);
    }

    private async Task<Dictionary<Guid, string>> LoadDrawingNamesAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        IPaginate<Drawing> drawings = await _drawingRepository.GetListAsync(
            predicate: d => d.TenantId == tenantId && d.ProjectId == projectId,
            index: 0,
            size: ExportPageSize,
            cancellationToken: cancellationToken
        );

        return drawings.Items.ToDictionary(d => d.Id, d => d.FileName);
    }

    private static int WritePuantajTitleSection(IXLWorksheet sheet, string tenantName, string projectName)
    {
        sheet.Cell(1, 1).Value = "Firma";
        sheet.Cell(1, 2).Value = tenantName;
        sheet.Cell(2, 1).Value = "Proje";
        sheet.Cell(2, 2).Value = projectName;
        sheet.Cell(3, 1).Value = "Dışa aktarma tarihi";
        sheet.Cell(3, 2).Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        var titleRange = sheet.Range(1, 1, 3, 1);
        titleRange.Style.Font.Bold = true;

        return 5;
    }

    private static void WriteHeader(IXLWorksheet sheet, int row, params string[] headers)
    {
        for (var col = 0; col < headers.Length; col++)
            sheet.Cell(row, col + 1).Value = headers[col];

        var headerRange = sheet.Range(row, 1, row, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");
    }

    private static void WriteHeader(IXLWorksheet sheet, params string[] headers) => WriteHeader(sheet, 1, headers);

    private static void FormatSheet(IXLWorksheet sheet, int freezeRow = 1)
    {
        sheet.Columns().AdjustToContents(1, 80);
        sheet.SheetView.FreezeRows(freezeRow);
    }

    private static ExportFileResponse ToResponse(XLWorkbook workbook, string fileName)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new ExportFileResponse
        {
            FileContent = stream.ToArray(),
            FileName = fileName
        };
    }

    private static string BuildProjectFileName(string prefix, Project project)
    {
        var slug = string.IsNullOrWhiteSpace(project.Code) ? project.Name : project.Code!;
        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_').ToArray());
        if (string.IsNullOrWhiteSpace(slug))
            slug = "proje";

        return $"{prefix}_{slug}_{DateTime.Now:yyyyMMdd}.xlsx";
    }
}
