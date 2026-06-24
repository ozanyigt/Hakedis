using Application.Features.HakedisDeductionLines.Commands.Create;
using Application.Features.HakedisDeductionLines.Commands.Delete;
using Application.Features.HakedisDeductionLines.Commands.Update;
using Application.Features.HakedisDeductionLines.Queries.GetById;
using Application.Features.HakedisDeductionLines.Queries.GetListByDynamic;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HakedisDeductionLinesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedHakedisDeductionLineResponse>> Add(
        [FromBody] CreateHakedisDeductionLineCommand command
    )
    {
        CreatedHakedisDeductionLineResponse response = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedHakedisDeductionLineResponse>> Update(
        [FromBody] UpdateHakedisDeductionLineCommand command
    )
    {
        UpdatedHakedisDeductionLineResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedHakedisDeductionLineResponse>> Delete([FromRoute] Guid id)
    {
        DeletedHakedisDeductionLineResponse response = await Mediator.Send(
            new DeleteHakedisDeductionLineCommand { Id = id }
        );
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdHakedisDeductionLineResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdHakedisDeductionLineResponse response = await Mediator.Send(
            new GetByIdHakedisDeductionLineQuery { Id = id }
        );
        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic(
        [FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery dynamic
    )
    {
        GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto> response = await Mediator.Send(
            new GetListByDynamicHakedisDeductionLineQuery { PageRequest = pageRequest, Dynamic = dynamic }
        );
        return Ok(response);
    }
}
