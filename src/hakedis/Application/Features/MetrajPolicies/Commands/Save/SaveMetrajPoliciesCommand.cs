using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;
using Application.Features.MetrajPolicies.Queries.GetByTenant;

namespace Application.Features.MetrajPolicies.Commands.Save;

public class SaveMetrajPoliciesCommand
    : IRequest<IReadOnlyList<MetrajPolicyDto>>,
        ISecuredRequest,
        ILoggableRequest,
        ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public IList<MetrajPolicyItemDto> Policies { get; set; } = [];

    public string[] Roles =>
        [
            Admin,
            Write,
            Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims.Update
        ];

    public class Handler : IRequestHandler<SaveMetrajPoliciesCommand, IReadOnlyList<MetrajPolicyDto>>
    {
        private readonly IMetrajPolicyRepository _metrajPolicyRepository;

        public Handler(IMetrajPolicyRepository metrajPolicyRepository)
        {
            _metrajPolicyRepository = metrajPolicyRepository;
        }

        public async Task<IReadOnlyList<MetrajPolicyDto>> Handle(
            SaveMetrajPoliciesCommand request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<MetrajPolicy> existing = await _metrajPolicyRepository.GetListAsync(
                predicate: policy => policy.TenantId == request.TenantId,
                index: 0,
                size: 200,
                cancellationToken: cancellationToken
            );

            Dictionary<string, MetrajPolicy> byCode = existing.Items.ToDictionary(
                policy => policy.Code,
                StringComparer.OrdinalIgnoreCase
            );

            HashSet<string> incomingCodes = new(StringComparer.OrdinalIgnoreCase);

            foreach (MetrajPolicyItemDto item in request.Policies)
            {
                if (string.IsNullOrWhiteSpace(item.Code) || string.IsNullOrWhiteSpace(item.Title))
                    continue;

                string code = item.Code.Trim().ToUpperInvariant();
                incomingCodes.Add(code);

                if (byCode.TryGetValue(code, out MetrajPolicy? entity))
                {
                    entity.Title = item.Title.Trim();
                    entity.Body = item.Body?.Trim() ?? string.Empty;
                    entity.IsActive = item.IsActive;
                    entity.Version += 1;
                    await _metrajPolicyRepository.UpdateAsync(entity);
                }
                else
                {
                    MetrajPolicy created =
                        new()
                        {
                            Id = Guid.NewGuid(),
                            TenantId = request.TenantId,
                            Code = code,
                            Title = item.Title.Trim(),
                            Body = item.Body?.Trim() ?? string.Empty,
                            Version = 1,
                            IsActive = item.IsActive
                        };
                    await _metrajPolicyRepository.AddAsync(created);
                    byCode[code] = created;
                }
            }

            foreach (MetrajPolicy leftover in existing.Items.Where(policy => !incomingCodes.Contains(policy.Code)))
            {
                leftover.IsActive = false;
                await _metrajPolicyRepository.UpdateAsync(leftover);
            }

            return byCode
                .Values.Where(policy => incomingCodes.Contains(policy.Code))
                .OrderBy(policy => policy.Code)
                .Select(policy => new MetrajPolicyDto
                {
                    Id = policy.Id,
                    TenantId = policy.TenantId,
                    Code = policy.Code,
                    Title = policy.Title,
                    Body = policy.Body,
                    Version = policy.Version,
                    IsActive = policy.IsActive
                })
                .ToList();
        }
    }
}

public class MetrajPolicyItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
