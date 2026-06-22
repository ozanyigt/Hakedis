using Application.Features.SubscriptionPlans.Commands.Create;
using Application.Features.SubscriptionPlans.Commands.Delete;
using Application.Features.SubscriptionPlans.Commands.Update;
using Application.Features.SubscriptionPlans.Queries.GetById;
using Application.Features.SubscriptionPlans.Queries.GetList;
using Application.Features.SubscriptionPlans.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionPlansController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedSubscriptionPlanResponse>> Add([FromBody] CreateSubscriptionPlanCommand command)
    {
        CreatedSubscriptionPlanResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedSubscriptionPlanResponse>> Update([FromBody] UpdateSubscriptionPlanCommand command)
    {
        UpdatedSubscriptionPlanResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedSubscriptionPlanResponse>> Delete([FromRoute] Guid id)
    {
        DeleteSubscriptionPlanCommand command = new() { Id = id };

        DeletedSubscriptionPlanResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdSubscriptionPlanResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdSubscriptionPlanQuery query = new() { Id = id };

        GetByIdSubscriptionPlanResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListSubscriptionPlanListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListSubscriptionPlanQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListSubscriptionPlanListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicSubscriptionPlanQuery getListByDynamicSubscriptionPlanQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicSubscriptionPlanListItemDto> response = await Mediator.Send(getListByDynamicSubscriptionPlanQuery);
        return Ok(response);
    }
}