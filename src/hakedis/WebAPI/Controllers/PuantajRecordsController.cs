using Application.Features.PuantajRecords.Commands.Create;
using Application.Features.PuantajRecords.Commands.Delete;
using Application.Features.PuantajRecords.Commands.Update;
using Application.Features.PuantajRecords.Queries.GetById;
using Application.Features.PuantajRecords.Queries.GetList;
using Application.Features.PuantajRecords.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PuantajRecordsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedPuantajRecordResponse>> Add([FromBody] CreatePuantajRecordCommand command)
    {
        CreatedPuantajRecordResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedPuantajRecordResponse>> Update([FromBody] UpdatePuantajRecordCommand command)
    {
        UpdatedPuantajRecordResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedPuantajRecordResponse>> Delete([FromRoute] Guid id, [FromQuery] Guid? tenantId)
    {
        DeletePuantajRecordCommand command = new() { Id = id, TenantId = tenantId };

        DeletedPuantajRecordResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdPuantajRecordResponse>> GetById([FromRoute] Guid id, [FromQuery] Guid? tenantId)
    {
        GetByIdPuantajRecordQuery query = new() { Id = id, TenantId = tenantId };

        GetByIdPuantajRecordResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListPuantajRecordListItemDto>>> GetList(
        [FromQuery] PageRequest pageRequest, [FromQuery] Guid? tenantId,
        [FromQuery] Guid? projectId, [FromQuery] Guid? siteId)
    {
        GetListPuantajRecordQuery query = new()
        {
            PageRequest = pageRequest, TenantId = tenantId, ProjectId = projectId, SiteId = siteId
        };

        GetListResponse<GetListPuantajRecordListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicPuantajRecordQuery getListByDynamicPuantajRecordQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicPuantajRecordListItemDto> response = await Mediator.Send(getListByDynamicPuantajRecordQuery);
        return Ok(response);
    }
}