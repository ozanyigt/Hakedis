using Application.Features.MetrajRuleTemplates.Commands.Create;
using Application.Features.MetrajRuleTemplates.Commands.Delete;
using Application.Features.MetrajRuleTemplates.Commands.Update;
using Application.Features.MetrajRuleTemplates.Queries.GetById;
using Application.Features.MetrajRuleTemplates.Queries.GetList;
using Application.Features.MetrajRuleTemplates.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MetrajRuleTemplatesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedMetrajRuleTemplateResponse>> Add([FromBody] CreateMetrajRuleTemplateCommand command)
    {
        CreatedMetrajRuleTemplateResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedMetrajRuleTemplateResponse>> Update([FromBody] UpdateMetrajRuleTemplateCommand command)
    {
        UpdatedMetrajRuleTemplateResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedMetrajRuleTemplateResponse>> Delete([FromRoute] Guid id)
    {
        DeleteMetrajRuleTemplateCommand command = new() { Id = id };

        DeletedMetrajRuleTemplateResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdMetrajRuleTemplateResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdMetrajRuleTemplateQuery query = new() { Id = id };

        GetByIdMetrajRuleTemplateResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListMetrajRuleTemplateListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListMetrajRuleTemplateQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListMetrajRuleTemplateListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicMetrajRuleTemplateQuery getListByDynamicMetrajRuleTemplateQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto> response = await Mediator.Send(getListByDynamicMetrajRuleTemplateQuery);
        return Ok(response);
    }
}