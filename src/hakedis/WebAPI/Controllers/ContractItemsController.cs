using Application.Features.ContractItems.Commands.Create;
using Application.Features.ContractItems.Commands.Delete;
using Application.Features.ContractItems.Commands.Update;
using Application.Features.ContractItems.Queries.GetById;
using Application.Features.ContractItems.Queries.GetList;
using Application.Features.ContractItems.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContractItemsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedContractItemResponse>> Add([FromBody] CreateContractItemCommand command)
    {
        CreatedContractItemResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedContractItemResponse>> Update([FromBody] UpdateContractItemCommand command)
    {
        UpdatedContractItemResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedContractItemResponse>> Delete([FromRoute] Guid id)
    {
        DeleteContractItemCommand command = new() { Id = id };

        DeletedContractItemResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdContractItemResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdContractItemQuery query = new() { Id = id };

        GetByIdContractItemResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListContractItemListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListContractItemQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListContractItemListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicContractItemQuery getListByDynamicContractItemQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicContractItemListItemDto> response = await Mediator.Send(getListByDynamicContractItemQuery);
        return Ok(response);
    }
}