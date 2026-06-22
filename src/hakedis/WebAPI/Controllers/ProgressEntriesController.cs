using Application.Features.ProgressEntries.Commands.Create;
using Application.Features.ProgressEntries.Commands.Delete;
using Application.Features.ProgressEntries.Commands.Update;
using Application.Features.ProgressEntries.Queries.GetById;
using Application.Features.ProgressEntries.Queries.GetList;
using Application.Features.ProgressEntries.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProgressEntriesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedProgressEntryResponse>> Add([FromBody] CreateProgressEntryCommand command)
    {
        CreatedProgressEntryResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedProgressEntryResponse>> Update([FromBody] UpdateProgressEntryCommand command)
    {
        UpdatedProgressEntryResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedProgressEntryResponse>> Delete([FromRoute] Guid id)
    {
        DeleteProgressEntryCommand command = new() { Id = id };

        DeletedProgressEntryResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdProgressEntryResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdProgressEntryQuery query = new() { Id = id };

        GetByIdProgressEntryResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListProgressEntryListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListProgressEntryQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListProgressEntryListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicProgressEntryQuery getListByDynamicProgressEntryQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicProgressEntryListItemDto> response = await Mediator.Send(getListByDynamicProgressEntryQuery);
        return Ok(response);
    }
}