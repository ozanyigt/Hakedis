using Application.Features.Tenants.Commands.Create;
using Application.Features.Tenants.Commands.Delete;
using Application.Features.Tenants.Commands.Update;
using Application.Features.Tenants.Queries.GetById;
using Application.Features.Tenants.Queries.GetList;
using Application.Features.Tenants.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedTenantResponse>> Add([FromBody] CreateTenantCommand command)
    {
        CreatedTenantResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedTenantResponse>> Update([FromBody] UpdateTenantCommand command)
    {
        UpdatedTenantResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedTenantResponse>> Delete([FromRoute] Guid id)
    {
        DeleteTenantCommand command = new() { Id = id };

        DeletedTenantResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdTenantResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdTenantQuery query = new() { Id = id };

        GetByIdTenantResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListTenantListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListTenantQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListTenantListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicTenantQuery getListByDynamicTenantQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicTenantListItemDto> response = await Mediator.Send(getListByDynamicTenantQuery);
        return Ok(response);
    }
}