using Application.Features.Tenants.Constants;
using Application.Features.Tenants.Constants;
using Application.Features.Tenants.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Tenants.Constants.TenantsOperationClaims;

namespace Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand : IRequest<DeletedTenantResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, TenantsOperationClaims.Delete];

    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, DeletedTenantResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITenantRepository _tenantRepository;
        private readonly TenantBusinessRules _tenantBusinessRules;

        public DeleteTenantCommandHandler(IMapper mapper, ITenantRepository tenantRepository,
                                         TenantBusinessRules tenantBusinessRules)
        {
            _mapper = mapper;
            _tenantRepository = tenantRepository;
            _tenantBusinessRules = tenantBusinessRules;
        }

        public async Task<DeletedTenantResponse> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            Tenant? tenant = await _tenantRepository.GetAsync(predicate: t => t.Id == request.Id, cancellationToken: cancellationToken);
            await _tenantBusinessRules.TenantShouldExistWhenSelected(tenant);

            await _tenantRepository.DeleteAsync(tenant!);

            DeletedTenantResponse response = _mapper.Map<DeletedTenantResponse>(tenant);
            return response;
        }
    }
}