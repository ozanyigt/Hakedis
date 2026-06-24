using Application.Features.MetrajResults.Constants;
using Application.Features.Drawings.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.MetrajCalculation;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;
using DrawingsClaims = Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.MetrajResults.Commands.Calculate;

public class CalculateMetrajCommand : IRequest<CalculateMetrajResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
  public required Guid DrawingId { get; set; }

  public string[] Roles => [Admin, Write, MetrajResultsOperationClaims.Create, DrawingsClaims.Write];

  public class CalculateMetrajCommandHandler : IRequestHandler<CalculateMetrajCommand, CalculateMetrajResponse>
  {
    private readonly IDrawingRepository _drawingRepository;
    private readonly IMetrajResultRepository _metrajResultRepository;
    private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
    private readonly IProjectMetrajLayerMappingRepository _projectMetrajLayerMappingRepository;
    private readonly IContractItemRepository _contractItemRepository;
    private readonly IMetrajCalculationService _metrajCalculationService;
    private readonly DrawingBusinessRules _drawingBusinessRules;

    public CalculateMetrajCommandHandler(
      IDrawingRepository drawingRepository,
      IMetrajResultRepository metrajResultRepository,
      IMetrajRuleTemplateRepository metrajRuleTemplateRepository,
      IProjectMetrajLayerMappingRepository projectMetrajLayerMappingRepository,
      IContractItemRepository contractItemRepository,
      IMetrajCalculationService metrajCalculationService,
      DrawingBusinessRules drawingBusinessRules
    )
    {
      _drawingRepository = drawingRepository;
      _metrajResultRepository = metrajResultRepository;
      _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
      _projectMetrajLayerMappingRepository = projectMetrajLayerMappingRepository;
      _contractItemRepository = contractItemRepository;
      _metrajCalculationService = metrajCalculationService;
      _drawingBusinessRules = drawingBusinessRules;
    }

    public async Task<CalculateMetrajResponse> Handle(CalculateMetrajCommand request, CancellationToken cancellationToken)
    {
      Drawing? drawing = await _drawingRepository.GetAsync(
        predicate: d => d.Id == request.DrawingId,
        cancellationToken: cancellationToken
      );
      await _drawingBusinessRules.DrawingShouldExistWhenSelected(drawing);

      drawing!.Status = DrawingStatus.Parsing;
      await _drawingRepository.UpdateAsync(drawing);

      try
      {
        IReadOnlyList<MetrajKalemRule> rules = await ResolveRulesAsync(drawing, cancellationToken);

        MetrajCalculationResultDto calculation = await _metrajCalculationService.CalculateAsync(
          new MetrajCalculationRequest
          {
            FilePath = drawing.FilePath,
            FileExtension = drawing.FileExtension,
            Rules = rules,
            FloorName = null,
            SpaceName = drawing.FileName
          },
          cancellationToken
        );

        if (!calculation.Success)
        {
          drawing.Status = DrawingStatus.Failed;
          drawing.ParseErrorMessage = calculation.ErrorMessage;
          drawing.ParsedAt = DateTime.UtcNow;
          await _drawingRepository.UpdateAsync(drawing);

          return new CalculateMetrajResponse
          {
            DrawingId = drawing.Id,
            Status = drawing.Status,
            ErrorMessage = calculation.ErrorMessage,
            Results = []
          };
        }

        await DeleteExistingResultsAsync(drawing.Id, cancellationToken);

        Dictionary<MetrajKalemType, MeasurementUnit> units = await GetContractUnitsAsync(drawing, cancellationToken);
        DateTime calculatedAt = DateTime.UtcNow;
        List<CalculatedMetrajItemDto> savedItems = [];

        foreach (MetrajCalculationItemDto item in calculation.Items)
        {
          MeasurementUnit unit = units.TryGetValue(item.KalemType, out MeasurementUnit contractUnit)
            ? contractUnit
            : item.Unit;

          MetrajResult entity = new()
          {
            Id = Guid.NewGuid(),
            TenantId = drawing.TenantId,
            ProjectId = drawing.ProjectId,
            SiteId = drawing.SiteId,
            DrawingId = drawing.Id,
            KalemType = item.KalemType,
            Unit = unit,
            Quantity = item.Quantity,
            FloorName = item.FloorName,
            SpaceName = item.SpaceName ?? drawing.FileName,
            CalculatedAt = calculatedAt,
            Notes = item.Notes
          };

          await _metrajResultRepository.AddAsync(entity);

          savedItems.Add(
            new CalculatedMetrajItemDto
            {
              Id = entity.Id,
              KalemType = entity.KalemType,
              Unit = entity.Unit,
              Quantity = entity.Quantity,
              FloorName = entity.FloorName,
              SpaceName = entity.SpaceName,
              Notes = entity.Notes
            }
          );
        }

        drawing.Status = DrawingStatus.Parsed;
        drawing.ParseErrorMessage = null;
        drawing.ParsedAt = calculatedAt;
        await _drawingRepository.UpdateAsync(drawing);

        return new CalculateMetrajResponse
        {
          DrawingId = drawing.Id,
          Status = drawing.Status,
          DrawingUnitNote = calculation.DrawingUnitNote,
          Results = savedItems
        };
      }
      catch (Exception ex)
      {
        drawing.Status = DrawingStatus.Failed;
        drawing.ParseErrorMessage = ex.Message;
        drawing.ParsedAt = DateTime.UtcNow;
        await _drawingRepository.UpdateAsync(drawing);
        throw;
      }
    }

    private async Task<IReadOnlyList<MetrajKalemRule>> ResolveRulesAsync(
      Drawing drawing,
      CancellationToken cancellationToken
    )
    {
      IPaginate<MetrajRuleTemplate> templates = await _metrajRuleTemplateRepository.GetListAsync(
        predicate: t => t.TenantId == drawing.TenantId && t.IsActive,
        index: 0,
        size: 100,
        cancellationToken: cancellationToken
      );

      IReadOnlyList<MetrajKalemRule> fallbackRules =
        templates.Items.Count > 0
          ? templates.Items.Select(MetrajCalculationDefaults.FromTemplate).ToList()
          : MetrajCalculationDefaults.GetDefaultRules();

      IPaginate<ProjectMetrajLayerMapping> projectMappings =
        await _projectMetrajLayerMappingRepository.GetListAsync(
          predicate: mapping => mapping.ProjectId == drawing.ProjectId,
          index: 0,
          size: 20,
          cancellationToken: cancellationToken
        );

      if (!MetrajCalculationRuleBuilder.HasProjectLayerMappings(projectMappings.Items))
        return fallbackRules;

      return MetrajCalculationRuleBuilder.MergeProjectLayerMappings(projectMappings.Items, fallbackRules);
    }

    private async Task<Dictionary<MetrajKalemType, MeasurementUnit>> GetContractUnitsAsync(
      Drawing drawing,
      CancellationToken cancellationToken
    )
    {
      IPaginate<ContractItem> items = await _contractItemRepository.GetListAsync(
        predicate: c => c.ProjectId == drawing.ProjectId,
        index: 0,
        size: 20,
        cancellationToken: cancellationToken
      );

      return items.Items.ToDictionary(i => i.KalemType, i => i.Unit);
    }

    private async Task DeleteExistingResultsAsync(Guid drawingId, CancellationToken cancellationToken)
    {
      IPaginate<MetrajResult> existing = await _metrajResultRepository.GetListAsync(
        predicate: r => r.DrawingId == drawingId,
        index: 0,
        size: 500,
        cancellationToken: cancellationToken
      );

      foreach (MetrajResult result in existing.Items)
        await _metrajResultRepository.DeleteAsync(result);
    }
  }
}
