using Application.Features.Workers.Commands.Create;
using Application.Features.Workers.Commands.Delete;
using Application.Features.Workers.Commands.Update;
using Application.Features.Workers.Queries.GetById;
using Application.Features.Workers.Queries.GetList;
using Application.Features.Workers.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkersController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedWorkerResponse>> Add([FromBody] CreateWorkerCommand command)
    {
        CreatedWorkerResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedWorkerResponse>> Update([FromBody] UpdateWorkerCommand command)
    {
        UpdatedWorkerResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedWorkerResponse>> Delete([FromRoute] Guid id)
    {
        DeleteWorkerCommand command = new() { Id = id };

        DeletedWorkerResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdWorkerResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdWorkerQuery query = new() { Id = id };

        GetByIdWorkerResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListWorkerListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListWorkerQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListWorkerListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicWorkerQuery getListByDynamicWorkerQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicWorkerListItemDto> response = await Mediator.Send(getListByDynamicWorkerQuery);
        return Ok(response);
    }
}