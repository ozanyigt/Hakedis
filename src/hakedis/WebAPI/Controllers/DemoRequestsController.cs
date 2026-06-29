using Application.Features.DemoRequests.Commands.Create;
using Application.Features.DemoRequests.Commands.UpdateStatus;
using Application.Features.DemoRequests.Queries.GetList;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DemoRequestsController : BaseController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<CreatedDemoRequestResponse>> Create(
        [FromBody] CreateDemoRequestCommand command,
        CancellationToken cancellationToken
    )
    {
        CreatedDemoRequestResponse response = await Mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListDemoRequestListItemDto>>> GetList(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken
    )
    {
        GetListResponse<GetListDemoRequestListItemDto> response = await Mediator.Send(
            new GetListDemoRequestQuery { PageRequest = pageRequest },
            cancellationToken
        );
        return Ok(response);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<UpdatedDemoRequestStatusResponse>> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateDemoRequestStatusBody body,
        CancellationToken cancellationToken
    )
    {
        UpdatedDemoRequestStatusResponse response = await Mediator.Send(
            new UpdateDemoRequestStatusCommand { Id = id, Status = body.Status },
            cancellationToken
        );
        return Ok(response);
    }
}

public class UpdateDemoRequestStatusBody
{
    public DemoRequestStatus Status { get; set; }
}
