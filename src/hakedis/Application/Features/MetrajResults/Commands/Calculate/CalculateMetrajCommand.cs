using Application.Features.MetrajResults.Constants;
using Application.Features.Drawings.Rules;
using Application.Services.MetrajCalculation;
using Application.Services.MetrajJudgment;
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
    private readonly IMetrajPolicyRepository _metrajPolicyRepository;
    private readonly IMetrajCalculationService _metrajCalculationService;
    private readonly IMetrajJudgmentService _metrajJudgmentService;
    private readonly DrawingBusinessRules _drawingBusinessRules;

    public CalculateMetrajCommandHandler(
      IDrawingRepository drawingRepository,
      IMetrajResultRepository metrajResultRepository,
      IMetrajRuleTemplateRepository metrajRuleTemplateRepository,
      IProjectMetrajLayerMappingRepository projectMetrajLayerMappingRepository,
      IContractItemRepository contractItemRepository,
      IMetrajPolicyRepository metrajPolicyRepository,
      IMetrajCalculationService metrajCalculationService,
      IMetrajJudgmentService metrajJudgmentService,
      DrawingBusinessRules drawingBusinessRules
    )
    {
      _drawingRepository = drawingRepository;
      _metrajResultRepository = metrajResultRepository;
      _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
      _projectMetrajLayerMappingRepository = projectMetrajLayerMappingRepository;
      _contractItemRepository = contractItemRepository;
      _metrajPolicyRepository = metrajPolicyRepository;
      _metrajCalculationService = metrajCalculationService;
      _metrajJudgmentService = metrajJudgmentService;
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
        List<MetrajResult> savedEntities = [];

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
            GrossQuantity = item.Quantity,
            Quantity = item.Quantity,
            SuggestedQuantity = item.Quantity,
            ApprovalStatus = MetrajApprovalStatus.Pending,
            IsLocked = false,
            FloorName = item.FloorName,
            SpaceName = item.SpaceName ?? drawing.FileName,
            CalculatedAt = calculatedAt,
            Notes = item.Notes
          };

          await _metrajResultRepository.AddAsync(entity);
          savedEntities.Add(entity);
        }

        IReadOnlyList<MetrajPolicy> policies = await EnsureDefaultPoliciesAsync(drawing.TenantId, cancellationToken);

        MetrajJudgmentResult judgment = await _metrajJudgmentService.JudgeAsync(
          new MetrajJudgmentRequest
          {
            DrawingId = drawing.Id,
            DrawingUnitNote = calculation.DrawingUnitNote,
            Policies = policies
              .Select(policy => new MetrajPolicySnippetDto
              {
                Code = policy.Code,
                Title = policy.Title,
                Body = policy.Body
              })
              .ToList(),
            Layers = calculation
              .Layers.Select(layer => new MetrajLayerSummaryDto
              {
                Name = layer.Name,
                EntityCount = layer.EntityCount,
                ClosedArea = layer.ClosedArea,
                LineLength = layer.LineLength
              })
              .ToList(),
            Items = savedEntities
              .Select(entity => new MetrajJudgmentItemRequest
              {
                MetrajResultId = entity.Id,
                KalemType = entity.KalemType,
                Unit = entity.Unit,
                GrossQuantity = entity.GrossQuantity,
                FloorName = entity.FloorName,
                SpaceName = entity.SpaceName,
                Notes = entity.Notes
              })
              .ToList()
          },
          cancellationToken
        );

        Dictionary<Guid, MetrajJudgmentItemResult> judgmentById = judgment.Items.ToDictionary(item => item.MetrajResultId);
        List<CalculatedMetrajItemDto> savedItems = [];

        foreach (MetrajResult entity in savedEntities)
        {
          if (judgmentById.TryGetValue(entity.Id, out MetrajJudgmentItemResult? itemJudgment))
          {
            entity.JudgmentDecision = itemJudgment.Decision;
            entity.JudgmentReason = itemJudgment.Reason;
            entity.PolicyRef = itemJudgment.PolicyRef;
            entity.AiConfidence = itemJudgment.Confidence;
            entity.SuggestedQuantity = itemJudgment.SuggestedQuantity ?? entity.GrossQuantity;
            entity.ApprovalStatus = MetrajApprovalStatus.AiSuggested;
            // Quantity brüt kalır; onayda nihai değer yazılır
            entity.Quantity = entity.GrossQuantity;
          }

          await _metrajResultRepository.UpdateAsync(entity);

          savedItems.Add(MapItem(entity));
        }

        drawing.Status = DrawingStatus.PendingReview;
        drawing.ParseErrorMessage = judgment.ErrorMessage;
        drawing.ParsedAt = calculatedAt;
        await _drawingRepository.UpdateAsync(drawing);

        return new CalculateMetrajResponse
        {
          DrawingId = drawing.Id,
          Status = drawing.Status,
          DrawingUnitNote = calculation.DrawingUnitNote,
          JudgmentNote = judgment.ErrorMessage,
          UsedAi = judgment.UsedAi,
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

    private async Task<IReadOnlyList<MetrajPolicy>> EnsureDefaultPoliciesAsync(
      Guid tenantId,
      CancellationToken cancellationToken
    )
    {
      IPaginate<MetrajPolicy> existing = await _metrajPolicyRepository.GetListAsync(
        predicate: policy => policy.TenantId == tenantId && policy.IsActive,
        index: 0,
        size: 100,
        cancellationToken: cancellationToken
      );

      if (existing.Items.Count > 0)
        return existing.Items.ToList();

      List<MetrajPolicy> defaults =
      [
        new()
        {
          Id = Guid.NewGuid(),
          TenantId = tenantId,
          Code = "K-12",
          Title = "Kırık / kesik kiriş",
          Body =
            "Süreksiz, kopuk veya hasarlı görünen kiriş hatları metraja dahil edilmez. Bu durumda decision=ignore ve suggestedQuantity=0 olmalıdır.",
          Version = 1,
          IsActive = true
        },
        new()
        {
          Id = Guid.NewGuid(),
          TenantId = tenantId,
          Code = "K-01",
          Title = "Küçük niş ihmal",
          Body = "0.50 m² altındaki niş / girinti alanları ihmal edilebilir (ignore).",
          Version = 1,
          IsActive = true
        },
        new()
        {
          Id = Guid.NewGuid(),
          TenantId = tenantId,
          Code = "K-20",
          Title = "Belirsizlikte inceleme",
          Body =
            "Katman adı, birim veya geometri belirsizse decision=needs_review seç; sayıyı uydurma.",
          Version = 1,
          IsActive = true
        }
      ];

      foreach (MetrajPolicy policy in defaults)
        await _metrajPolicyRepository.AddAsync(policy);

      return defaults;
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

    private static CalculatedMetrajItemDto MapItem(MetrajResult entity) =>
      new()
      {
        Id = entity.Id,
        KalemType = entity.KalemType,
        Unit = entity.Unit,
        Quantity = entity.Quantity,
        GrossQuantity = entity.GrossQuantity,
        SuggestedQuantity = entity.SuggestedQuantity,
        ApprovalStatus = entity.ApprovalStatus,
        JudgmentDecision = entity.JudgmentDecision,
        JudgmentReason = entity.JudgmentReason,
        PolicyRef = entity.PolicyRef,
        AiConfidence = entity.AiConfidence,
        IsLocked = entity.IsLocked,
        FloorName = entity.FloorName,
        SpaceName = entity.SpaceName,
        Notes = entity.Notes
      };
  }
}
