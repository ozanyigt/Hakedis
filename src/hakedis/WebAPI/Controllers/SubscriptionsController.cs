using Application.Features.Subscriptions.Commands.Create;
using Application.Features.Subscriptions.Commands.Delete;
using Application.Features.Subscriptions.Commands.Update;
using Application.Features.Subscriptions.Queries.GetById;
using Application.Features.Subscriptions.Queries.GetList;
using Application.Features.Subscriptions.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedSubscriptionResponse>> Add([FromBody] CreateSubscriptionCommand command)
    {
        CreatedSubscriptionResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedSubscriptionResponse>> Update([FromBody] UpdateSubscriptionCommand command)
    {
        UpdatedSubscriptionResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedSubscriptionResponse>> Delete([FromRoute] Guid id)
    {
        DeleteSubscriptionCommand command = new() { Id = id };

        DeletedSubscriptionResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdSubscriptionResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdSubscriptionQuery query = new() { Id = id };

        GetByIdSubscriptionResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListSubscriptionListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListSubscriptionQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListSubscriptionListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicSubscriptionQuery getListByDynamicSubscriptionQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicSubscriptionListItemDto> response = await Mediator.Send(getListByDynamicSubscriptionQuery);
        return Ok(response);
    }
}