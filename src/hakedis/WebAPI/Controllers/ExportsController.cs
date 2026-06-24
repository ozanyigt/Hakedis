using Application.Features.Exports.Queries.ExportExcel;
using Application.Services.Export;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExportsController : BaseController
{
    [HttpGet("projects")]
    public async Task<IActionResult> ExportProjects([FromQuery] Guid tenantId)
    {
        ExportFileResponse response = await Mediator.Send(
            new ExportExcelQuery { Kind = ExportExcelKind.Projects, TenantId = tenantId }
        );

        return File(response.FileContent, response.ContentType, response.FileName);
    }

    [HttpGet("puantaj")]
    public async Task<IActionResult> ExportPuantaj([FromQuery] Guid tenantId, [FromQuery] Guid projectId)
    {
        ExportFileResponse response = await Mediator.Send(
            new ExportExcelQuery
            {
                Kind = ExportExcelKind.Puantaj,
                TenantId = tenantId,
                ProjectId = projectId
            }
        );

        return File(response.FileContent, response.ContentType, response.FileName);
    }

    [HttpGet("metraj")]
    public async Task<IActionResult> ExportMetraj([FromQuery] Guid tenantId, [FromQuery] Guid projectId)
    {
        ExportFileResponse response = await Mediator.Send(
            new ExportExcelQuery
            {
                Kind = ExportExcelKind.Metraj,
                TenantId = tenantId,
                ProjectId = projectId
            }
        );

        return File(response.FileContent, response.ContentType, response.FileName);
    }

    [HttpGet("hakedis")]
    public async Task<IActionResult> ExportHakedis([FromQuery] Guid tenantId, [FromQuery] Guid projectId)
    {
        ExportFileResponse response = await Mediator.Send(
            new ExportExcelQuery
            {
                Kind = ExportExcelKind.Hakedis,
                TenantId = tenantId,
                ProjectId = projectId
            }
        );

        return File(response.FileContent, response.ContentType, response.FileName);
    }

    [HttpGet("contract-items")]
    public async Task<IActionResult> ExportContractItems([FromQuery] Guid tenantId, [FromQuery] Guid projectId)
    {
        ExportFileResponse response = await Mediator.Send(
            new ExportExcelQuery
            {
                Kind = ExportExcelKind.ContractItems,
                TenantId = tenantId,
                ProjectId = projectId
            }
        );

        return File(response.FileContent, response.ContentType, response.FileName);
    }
}
