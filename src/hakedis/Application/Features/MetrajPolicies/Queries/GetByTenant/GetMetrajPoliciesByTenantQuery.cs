using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajPolicies.Queries.GetByTenant;

public class GetMetrajPoliciesByTenantQuery : IRequest<IReadOnlyList<MetrajPolicyDto>>, ISecuredRequest
{
    public required Guid TenantId { get; set; }

    public string[] Roles => [Admin, Read];

    public class Handler : IRequestHandler<GetMetrajPoliciesByTenantQuery, IReadOnlyList<MetrajPolicyDto>>
    {
        private readonly IMetrajPolicyRepository _metrajPolicyRepository;

        public Handler(IMetrajPolicyRepository metrajPolicyRepository)
        {
            _metrajPolicyRepository = metrajPolicyRepository;
        }

        public async Task<IReadOnlyList<MetrajPolicyDto>> Handle(
            GetMetrajPoliciesByTenantQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<MetrajPolicy> page = await _metrajPolicyRepository.GetListAsync(
                predicate: policy => policy.TenantId == request.TenantId,
                index: 0,
                size: 200,
                cancellationToken: cancellationToken
            );

            return page
                .Items.OrderBy(policy => policy.Code)
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

public class MetrajPolicyDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsActive { get; set; }
}
