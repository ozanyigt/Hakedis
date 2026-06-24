using Application.Features.ProjectMetrajLayerMappings.Commands.Save;
using Application.Features.ProjectMetrajLayerMappings.Queries.GetByProject;
using Application.Features.Projects.Commands.Create;
using Application.Features.Projects.Commands.Delete;
using Application.Features.Projects.Commands.Update;
using Application.Features.Projects.Queries.GetById;
using Application.Features.Projects.Queries.GetList;
using Application.Features.Projects.Queries.GetListByDynamic;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedProjectResponse>> Add([FromBody] CreateProjectCommand command)
    {
        CreatedProjectResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedProjectResponse>> Update([FromBody] UpdateProjectCommand command)
    {
        UpdatedProjectResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedProjectResponse>> Delete([FromRoute] Guid id)
    {
        DeleteProjectCommand command = new() { Id = id };

        DeletedProjectResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdProjectResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdProjectQuery query = new() { Id = id };

        GetByIdProjectResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListProjectListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListProjectQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListProjectListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicProjectQuery getListByDynamicProjectQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicProjectListItemDto> response = await Mediator.Send(getListByDynamicProjectQuery);
        return Ok(response);
    }

    [HttpGet("{projectId:guid}/metraj-layer-mappings")]
    public async Task<ActionResult<IReadOnlyList<ProjectMetrajLayerMappingDto>>> GetMetrajLayerMappings(
        [FromRoute] Guid projectId,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<ProjectMetrajLayerMappingDto> response = await Mediator.Send(
            new GetProjectMetrajLayerMappingsQuery { ProjectId = projectId },
            cancellationToken
        );
        return Ok(response);
    }

    [HttpPut("{projectId:guid}/metraj-layer-mappings")]
    public async Task<ActionResult<IReadOnlyList<ProjectMetrajLayerMappingItemDto>>> SaveMetrajLayerMappings(
        [FromRoute] Guid projectId,
        [FromBody] IList<ProjectMetrajLayerMappingItemDto> mappings,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<ProjectMetrajLayerMappingItemDto> response = await Mediator.Send(
            new SaveProjectMetrajLayerMappingsCommand { ProjectId = projectId, Mappings = mappings },
            cancellationToken
        );
        return Ok(response);
    }
}