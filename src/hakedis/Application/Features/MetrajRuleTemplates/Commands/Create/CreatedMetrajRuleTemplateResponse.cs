using NArchitecture.Core.Application.Responses;
using Domain.Enums;

namespace Application.Features.MetrajRuleTemplates.Commands.Create;

public class CreatedMetrajRuleTemplateResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public MetrajKalemType KalemType { get; set; }
    public string LayerPatterns { get; set; }
    public string EntityTypes { get; set; }
    public MeasurementUnit Unit { get; set; }
    public decimal? DefaultThickness { get; set; }
    public decimal? DefaultHeight { get; set; }
    public bool DeductOpenings { get; set; }
    public string? OpeningLayerPatterns { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}