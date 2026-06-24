using Application.Features.ContractItems.Constants;
using Application.Features.Exports.Constants;
using Application.Features.HakedisPeriods.Constants;
using Application.Features.MetrajResults.Constants;
using Application.Features.Projects.Constants;
using Application.Features.PuantajRecords.Constants;
using Application.Services.Export;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using static Application.Features.Exports.Constants.ExportsOperationClaims;

namespace Application.Features.Exports.Queries.ExportExcel;

public enum ExportExcelKind
{
    Projects = 1,
    Puantaj = 2,
    Metraj = 3,
    Hakedis = 4,
    ContractItems = 5
}

public class ExportExcelQuery : IRequest<ExportFileResponse>, ISecuredRequest, ILoggableRequest
{
    public ExportExcelKind Kind { get; set; }
    public Guid TenantId { get; set; }
    public Guid? ProjectId { get; set; }

    public string[] Roles =>
    [
        Admin,
        Read,
        ProjectsOperationClaims.Admin,
        ProjectsOperationClaims.Read,
        PuantajRecordsOperationClaims.Admin,
        PuantajRecordsOperationClaims.Read,
        MetrajResultsOperationClaims.Admin,
        MetrajResultsOperationClaims.Read,
        HakedisPeriodsOperationClaims.Admin,
        HakedisPeriodsOperationClaims.Read,
        ContractItemsOperationClaims.Admin,
        ContractItemsOperationClaims.Read
    ];

    public class ExportExcelQueryHandler : IRequestHandler<ExportExcelQuery, ExportFileResponse>
    {
        private readonly IExcelExportService _excelExportService;

        public ExportExcelQueryHandler(IExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }

        public Task<ExportFileResponse> Handle(ExportExcelQuery request, CancellationToken cancellationToken)
        {
            return request.Kind switch
            {
                ExportExcelKind.Projects => _excelExportService.ExportProjectsAsync(request.TenantId, cancellationToken),
                ExportExcelKind.Puantaj => ExportForProject(
                    request,
                    _excelExportService.ExportPuantajAsync,
                    cancellationToken
                ),
                ExportExcelKind.Metraj => ExportForProject(
                    request,
                    _excelExportService.ExportMetrajAsync,
                    cancellationToken
                ),
                ExportExcelKind.Hakedis => ExportForProject(
                    request,
                    _excelExportService.ExportHakedisAsync,
                    cancellationToken
                ),
                ExportExcelKind.ContractItems => ExportForProject(
                    request,
                    _excelExportService.ExportContractItemsAsync,
                    cancellationToken
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(request.Kind), request.Kind, "Geçersiz export türü.")
            };
        }

        private static Task<ExportFileResponse> ExportForProject(
            ExportExcelQuery request,
            Func<Guid, Guid, CancellationToken, Task<ExportFileResponse>> exportFunc,
            CancellationToken cancellationToken
        )
        {
            if (!request.ProjectId.HasValue || request.ProjectId.Value == Guid.Empty)
                throw new ArgumentException("Bu export için proje seçilmelidir.", nameof(request.ProjectId));

            return exportFunc(request.TenantId, request.ProjectId.Value, cancellationToken);
        }
    }
}
