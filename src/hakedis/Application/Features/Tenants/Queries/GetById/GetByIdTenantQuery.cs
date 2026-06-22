using Application.Features.Tenants.Constants;
using Application.Features.Tenants.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Tenants.Constants.TenantsOperationClaims;

namespace Application.Features.Tenants.Queries.GetById;

public class GetByIdTenantQuery : IRequest<GetByIdTenantResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdTenantQueryHandler : IRequestHandler<GetByIdTenantQuery, GetByIdTenantResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITenantRepository _tenantRepository;
        private readonly TenantBusinessRules _tenantBusinessRules;

        public GetByIdTenantQueryHandler(IMapper mapper, ITenantRepository tenantRepository, TenantBusinessRules tenantBusinessRules)
        {
            _mapper = mapper;
            _tenantRepository = tenantRepository;
            _tenantBusinessRules = tenantBusinessRules;
        }

        public async Task<GetByIdTenantResponse> Handle(GetByIdTenantQuery request, CancellationToken cancellationToken)
        {
            Tenant? tenant = await _tenantRepository.GetAsync(predicate: t => t.Id == request.Id, cancellationToken: cancellationToken);
            await _tenantBusinessRules.TenantShouldExistWhenSelected(tenant);

            GetByIdTenantResponse response = _mapper.Map<GetByIdTenantResponse>(tenant);
            return response;
        }
    }
}