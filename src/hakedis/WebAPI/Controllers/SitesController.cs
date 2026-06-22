using Application.Features.Sites.Commands.Create;
using Application.Features.Sites.Commands.Delete;
using Application.Features.Sites.Commands.Update;
using Application.Features.Sites.Queries.GetById;
using Application.Features.Sites.Queries.GetList;
using Application.Features.Sites.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SitesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedSiteResponse>> Add([FromBody] CreateSiteCommand command)
    {
        CreatedSiteResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedSiteResponse>> Update([FromBody] UpdateSiteCommand command)
    {
        UpdatedSiteResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedSiteResponse>> Delete([FromRoute] Guid id)
    {
        DeleteSiteCommand command = new() { Id = id };

        DeletedSiteResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdSiteResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdSiteQuery query = new() { Id = id };

        GetByIdSiteResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListSiteListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListSiteQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListSiteListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicSiteQuery getListByDynamicSiteQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicSiteListItemDto> response = await Mediator.Send(getListByDynamicSiteQuery);
        return Ok(response);
    }
}