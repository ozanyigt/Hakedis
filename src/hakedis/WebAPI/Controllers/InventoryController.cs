using Application.Features.Inventory;
using Application.Services.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/inventory")]
[ApiController]
public class InventoryController : BaseController
{
    [HttpPost("materials")]
    public async Task<ActionResult<MaterialDto>> CreateMaterial(CreateMaterialCommand command) =>
        Ok(await Mediator.Send(command));

    [HttpPut("materials/{id:guid}")]
    public async Task<ActionResult<MaterialDto>> UpdateMaterial(Guid id, UpdateMaterialCommand command)
    {
        command.Id = id;
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("materials/{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteMaterial(Guid id, [FromQuery] Guid? tenantId) =>
        Ok(await Mediator.Send(new DeleteMaterialCommand { Id = id, TenantId = tenantId }));

    [HttpGet("materials")]
    public async Task<ActionResult<IReadOnlyList<MaterialDto>>> Materials(
        [FromQuery] Guid? tenantId, [FromQuery] bool includeInactive = false) =>
        Ok(await Mediator.Send(new GetMaterialsQuery { TenantId = tenantId, IncludeInactive = includeInactive }));

    [HttpGet("balances")]
    public async Task<ActionResult<IReadOnlyList<StockBalanceDto>>> Balances(
        [FromQuery] Guid? tenantId, [FromQuery] Guid? siteId, [FromQuery] Guid? materialId) =>
        Ok(await Mediator.Send(new GetStockBalancesQuery
        {
            TenantId = tenantId, SiteId = siteId, MaterialId = materialId
        }));

    [HttpGet("ledger")]
    public async Task<ActionResult<IReadOnlyList<StockTransactionDto>>> Ledger(
        [FromQuery] Guid? tenantId, [FromQuery] Guid? siteId,
        [FromQuery] Guid? materialId, [FromQuery] int take = 200) =>
        Ok(await Mediator.Send(new GetStockLedgerQuery
        {
            TenantId = tenantId, SiteId = siteId, MaterialId = materialId, Take = take
        }));

    [HttpPost("receipts")]
    public async Task<ActionResult<StockPostingResult>> Receipt(ReceiveStockCommand command) =>
        Ok(await Mediator.Send(command));

    [HttpPost("consumptions")]
    public async Task<ActionResult<StockPostingResult>> Consumption(ConsumeStockCommand command) =>
        Ok(await Mediator.Send(command));

    [HttpPost("transfers")]
    public async Task<ActionResult<StockPostingResult>> Transfer(TransferStockCommand command) =>
        Ok(await Mediator.Send(command));

    [HttpPost("adjustments")]
    public async Task<ActionResult<StockPostingResult>> Adjustment(AdjustStockCommand command) =>
        Ok(await Mediator.Send(command));
}
