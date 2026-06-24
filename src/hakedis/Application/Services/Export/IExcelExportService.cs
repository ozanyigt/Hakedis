namespace Application.Services.Export;

public interface IExcelExportService
{
    Task<ExportFileResponse> ExportProjectsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<ExportFileResponse> ExportPuantajAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
    Task<ExportFileResponse> ExportMetrajAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
    Task<ExportFileResponse> ExportHakedisAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
    Task<ExportFileResponse> ExportContractItemsAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);
}
