using Application.Features.MetrajResults.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Commands.Approve;

public class ApproveMetrajResultsCommand : IRequest<ApproveMetrajResultsResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid DrawingId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public IList<ApproveMetrajResultItemDto> Items { get; set; } = [];

    public string[] Roles => [Admin, Write, MetrajResultsOperationClaims.Update];

    public class ApproveMetrajResultsCommandHandler
        : IRequestHandler<ApproveMetrajResultsCommand, ApproveMetrajResultsResponse>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMetrajResultRepository _metrajResultRepository;

        public ApproveMetrajResultsCommandHandler(
            IDrawingRepository drawingRepository,
            IMetrajResultRepository metrajResultRepository
        )
        {
            _drawingRepository = drawingRepository;
            _metrajResultRepository = metrajResultRepository;
        }

        public async Task<ApproveMetrajResultsResponse> Handle(
            ApproveMetrajResultsCommand request,
            CancellationToken cancellationToken
        )
        {
            Drawing? drawing = await _drawingRepository.GetAsync(
                predicate: d => d.Id == request.DrawingId,
                cancellationToken: cancellationToken
            );

            if (drawing is null)
                return new ApproveMetrajResultsResponse
                {
                    DrawingId = request.DrawingId,
                    Success = false,
                    ErrorMessage = "Çizim bulunamadı."
                };

            IPaginate<MetrajResult> existing = await _metrajResultRepository.GetListAsync(
                predicate: r => r.DrawingId == request.DrawingId,
                index: 0,
                size: 500,
                cancellationToken: cancellationToken
            );

            Dictionary<Guid, ApproveMetrajResultItemDto> overrides = request.Items.ToDictionary(item => item.Id);
            DateTime reviewedAt = DateTime.UtcNow;
            int approvedCount = 0;

            foreach (MetrajResult entity in existing.Items)
            {
                if (entity.IsLocked)
                    continue;

                decimal finalQuantity = entity.SuggestedQuantity ?? entity.GrossQuantity;
                MetrajApprovalStatus status = MetrajApprovalStatus.Approved;

                if (overrides.TryGetValue(entity.Id, out ApproveMetrajResultItemDto? itemOverride))
                {
                    if (itemOverride.Reject)
                    {
                        status = MetrajApprovalStatus.Rejected;
                        finalQuantity = 0;
                    }
                    else if (itemOverride.ApprovedQuantity.HasValue)
                    {
                        finalQuantity = Math.Max(0, itemOverride.ApprovedQuantity.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(itemOverride.ReviewNote))
                    {
                        entity.JudgmentReason = string.IsNullOrWhiteSpace(entity.JudgmentReason)
                            ? itemOverride.ReviewNote
                            : $"{entity.JudgmentReason} | Onay notu: {itemOverride.ReviewNote}";
                    }
                }
                else if (entity.JudgmentDecision == MetrajJudgmentDecision.Ignore)
                {
                    finalQuantity = 0;
                }

                if (finalQuantity > entity.GrossQuantity)
                    finalQuantity = entity.GrossQuantity;

                entity.Quantity = finalQuantity;
                entity.ApprovalStatus = status;
                entity.IsLocked = status == MetrajApprovalStatus.Approved;
                entity.ReviewedByUserId = request.ReviewedByUserId;
                entity.ReviewedAt = reviewedAt;
                await _metrajResultRepository.UpdateAsync(entity);
                approvedCount++;
            }

            drawing.Status = DrawingStatus.Approved;
            drawing.ParseErrorMessage = null;
            await _drawingRepository.UpdateAsync(drawing);

            return new ApproveMetrajResultsResponse
            {
                DrawingId = drawing.Id,
                Success = true,
                Status = drawing.Status,
                ApprovedCount = approvedCount
            };
        }
    }
}

public class ApproveMetrajResultItemDto
{
    public Guid Id { get; set; }
    public decimal? ApprovedQuantity { get; set; }
    public bool Reject { get; set; }
    public string? ReviewNote { get; set; }
}
