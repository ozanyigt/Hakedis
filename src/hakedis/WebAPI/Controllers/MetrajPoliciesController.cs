using Application.Features.MetrajPolicies.Commands.Save;
using Application.Features.MetrajPolicies.Queries.GetByTenant;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MetrajPoliciesController : BaseController
{
    [HttpGet("by-tenant/{tenantId:guid}")]
    public async Task<ActionResult<IReadOnlyList<MetrajPolicyDto>>> GetByTenant([FromRoute] Guid tenantId)
    {
        IReadOnlyList<MetrajPolicyDto> response = await Mediator.Send(
            new GetMetrajPoliciesByTenantQuery { TenantId = tenantId }
        );
        return Ok(response);
    }

    [HttpPut("by-tenant/{tenantId:guid}")]
    public async Task<ActionResult<IReadOnlyList<MetrajPolicyDto>>> Save(
        [FromRoute] Guid tenantId,
        [FromBody] IList<MetrajPolicyItemDto> policies
    )
    {
        IReadOnlyList<MetrajPolicyDto> response = await Mediator.Send(
            new SaveMetrajPoliciesCommand { TenantId = tenantId, Policies = policies }
        );
        return Ok(response);
    }
}
