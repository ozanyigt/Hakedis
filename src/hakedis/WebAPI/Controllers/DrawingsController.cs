using Application.Features.Drawings.Commands.Create;
using Application.Features.Drawings.Commands.Delete;
using Application.Features.Drawings.Commands.Update;
using Application.Features.Drawings.Queries.GetById;
using Application.Features.Drawings.Queries.GetList;
using Application.Features.Drawings.Queries.GetListByDynamic;
using Application.Features.MetrajResults.Commands.Calculate;
using Domain.Enums;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DrawingsController : BaseController
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".dwg", ".dxf" };

    [HttpPost("files/upload")]
    [RequestSizeLimit(314_572_800)]
    [RequestFormLimits(MultipartBodyLengthLimit = 314_572_800)]
    public async Task<ActionResult<CreatedDrawingResponse>> Upload(
        [FromForm] Guid tenantId,
        [FromForm] Guid projectId,
        [FromForm] Guid? siteId,
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya gerekli." });

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { message = "Yalnızca DWG veya DXF dosyaları yüklenebilir." });

        string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "drawings");
        Directory.CreateDirectory(uploadsDir);

        string storedFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(uploadsDir, storedFileName);

        await using (FileStream stream = new(filePath, FileMode.Create))
            await file.CopyToAsync(stream, cancellationToken);

        CreateDrawingCommand command =
            new()
            {
                TenantId = tenantId,
                ProjectId = projectId,
                SiteId = siteId,
                FileName = file.FileName,
                FilePath = filePath,
                FileExtension = extension.TrimStart('.'),
                FileSizeBytes = file.Length,
                Status = DrawingStatus.Uploaded
            };

        CreatedDrawingResponse response = await Mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{id:guid}/calculate-metraj")]
    public async Task<ActionResult<CalculateMetrajResponse>> CalculateMetraj(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CalculateMetrajResponse response = await Mediator.Send(
            new CalculateMetrajCommand { DrawingId = id },
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CreatedDrawingResponse>> Add([FromBody] CreateDrawingCommand command)
    {
        CreatedDrawingResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedDrawingResponse>> Update([FromBody] UpdateDrawingCommand command)
    {
        UpdatedDrawingResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<DeletedDrawingResponse>> Delete([FromRoute] Guid id)
    {
        DeleteDrawingCommand command = new() { Id = id };

        DeletedDrawingResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetByIdDrawingResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdDrawingQuery query = new() { Id = id };

        GetByIdDrawingResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListDrawingListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListDrawingQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListDrawingListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("GetListByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamic)
    {
        GetListByDynamicDrawingQuery getListByDynamicDrawingQuery = new() { PageRequest = pageRequest, Dynamic = dynamic };
        GetListResponse<GetListByDynamicDrawingListItemDto> response = await Mediator.Send(getListByDynamicDrawingQuery);
        return Ok(response);
    }
}