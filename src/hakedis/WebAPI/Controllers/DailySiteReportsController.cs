using Application.Features.DailySiteReports;
using Application.Features.DailySiteReports.Commands;
using Application.Features.DailySiteReports.Queries;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailySiteReportsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<DailySiteReportDto>> Create([FromBody] CreateDailySiteReportCommand command)
    {
        DailySiteReportDto response = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DailySiteReportDto>> Update(
        [FromRoute] Guid id, [FromBody] UpdateDailySiteReportCommand command)
    {
        command.Id = id;
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new DeleteDailySiteReportCommand { Id = id, TenantId = tenantId }));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DailySiteReportDto>> GetById([FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new GetDailySiteReportByIdQuery { Id = id, TenantId = tenantId }));

    [HttpGet]
    public async Task<ActionResult<GetListResponse<DailySiteReportListItemDto>>> GetList(
        [FromQuery] PageRequest pageRequest,
        [FromQuery] Guid? tenantId,
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? siteId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] DailySiteReportStatus? status) =>
        Ok(await Mediator.Send(new GetListDailySiteReportQuery
        {
            PageRequest = pageRequest,
            TenantId = tenantId,
            ProjectId = projectId,
            SiteId = siteId,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status
        }));

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<DailySiteReportDto>> Submit([FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new SubmitDailySiteReportCommand { Id = id, TenantId = tenantId }));

    [HttpGet("workforce-preview")]
    public async Task<ActionResult<DailySiteReportWorkforceDto>> WorkforcePreview(
        [FromQuery] Guid? tenantId, [FromQuery] Guid projectId,
        [FromQuery] Guid siteId, [FromQuery] DateTime reportDate) =>
        Ok(await Mediator.Send(new PreviewDailySiteReportWorkforceQuery
        {
            TenantId = tenantId, ProjectId = projectId, SiteId = siteId, ReportDate = reportDate
        }));

    [HttpGet("{id:guid}/workforce-snapshot")]
    public async Task<ActionResult<DailySiteReportWorkforceDto>> WorkforceSnapshot(
        [FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new GetDailySiteReportWorkforceSnapshotQuery
        {
            ReportId = id, TenantId = tenantId
        }));

    [HttpGet("{id:guid}/materials")]
    public async Task<ActionResult<IReadOnlyList<DailySiteReportMaterialLineDto>>> Materials(
        [FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new GetDailySiteReportMaterialLinesQuery
        {
            ReportId = id, TenantId = tenantId
        }));

    [HttpPut("{id:guid}/materials")]
    public async Task<ActionResult<IReadOnlyList<DailySiteReportMaterialLineDto>>> ReplaceMaterials(
        [FromRoute] Guid id, [FromQuery] Guid? tenantId,
        [FromBody] IReadOnlyList<DailySiteReportMaterialLineInput> lines) =>
        Ok(await Mediator.Send(new ReplaceDailySiteReportMaterialLinesCommand
        {
            ReportId = id, TenantId = tenantId, Lines = lines
        }));

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<DailySiteReportDto>> Approve([FromRoute] Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new ApproveDailySiteReportCommand { Id = id, TenantId = tenantId }));

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<DailySiteReportDto>> Reject(
        [FromRoute] Guid id,
        [FromBody] RejectDailySiteReportRequest request,
        [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new RejectDailySiteReportCommand
        {
            Id = id,
            TenantId = tenantId,
            Reason = request.Reason
        }));

    [HttpPost("{id:guid}/photos")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DailySiteReportPhotoDto>> UploadPhoto(
        [FromRoute] Guid id,
        [FromForm] IFormFile file,
        [FromForm] string? description,
        [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new UploadDailySiteReportPhotoCommand
        {
            ReportId = id,
            TenantId = tenantId,
            File = file,
            Description = description
        }));

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<ActionResult<Guid>> DeletePhoto(
        [FromRoute] Guid id, [FromRoute] Guid photoId, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new DeleteDailySiteReportPhotoCommand
        {
            ReportId = id,
            PhotoId = photoId,
            TenantId = tenantId
        }));
}

public sealed class RejectDailySiteReportRequest
{
    public string? Reason { get; set; }
}
