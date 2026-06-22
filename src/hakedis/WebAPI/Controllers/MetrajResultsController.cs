using Application.Features.MetrajResults.Commands.Create;
using Application.Features.MetrajResults.Commands.Delete;
using Application.Features.MetrajResults.Commands.Update;
using Application.Features.MetrajResults.Queries.GetById;
using Application.Features.MetrajResults.Queries.GetList;
using Application.Features.MetrajResults.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MetrajResultsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedMetrajResultResponse>> Add([FromBody] CreateMetrajResultCommand command)
    {
        CreatedMetrajResultResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedMetrajResultResponse>> Update([FromBody] UpdateMetrajResultCommand command)
    {
        UpdatedMetrajResultResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedMetrajResultResponse>> Delete([FromRoute] Guid id)
    {
        DeleteMetrajResultCommand command = new() { Id = id };

        DeletedMetrajResultResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdMetrajResultResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdMetrajResultQuery query = new() { Id = id };

        GetByIdMetrajResultResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListMetrajResultListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListMetrajResultQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListMetrajResultListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicMetrajResultQuery getListByDynamicMetrajResultQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicMetrajResultListItemDto> response = await Mediator.Send(getListByDynamicMetrajResultQuery);
        return Ok(response);
    }
}