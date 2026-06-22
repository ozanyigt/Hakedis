using Application.Features.HakedisPeriods.Commands.Create;
using Application.Features.HakedisPeriods.Commands.Delete;
using Application.Features.HakedisPeriods.Commands.Update;
using Application.Features.HakedisPeriods.Queries.GetById;
using Application.Features.HakedisPeriods.Queries.GetList;
using Application.Features.HakedisPeriods.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HakedisPeriodsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedHakedisPeriodResponse>> Add([FromBody] CreateHakedisPeriodCommand command)
    {
        CreatedHakedisPeriodResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedHakedisPeriodResponse>> Update([FromBody] UpdateHakedisPeriodCommand command)
    {
        UpdatedHakedisPeriodResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedHakedisPeriodResponse>> Delete([FromRoute] Guid id)
    {
        DeleteHakedisPeriodCommand command = new() { Id = id };

        DeletedHakedisPeriodResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdHakedisPeriodResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdHakedisPeriodQuery query = new() { Id = id };

        GetByIdHakedisPeriodResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListHakedisPeriodListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListHakedisPeriodQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListHakedisPeriodListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicHakedisPeriodQuery getListByDynamicHakedisPeriodQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicHakedisPeriodListItemDto> response = await Mediator.Send(getListByDynamicHakedisPeriodQuery);
        return Ok(response);
    }
}