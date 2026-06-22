using Application.Features.Tenants.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Tenants.Constants.TenantsOperationClaims;

namespace Application.Features.Tenants.Queries.GetList;

public class GetListTenantQuery : IRequest<GetListResponse<GetListTenantListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListTenantQueryHandler : IRequestHandler<GetListTenantQuery, GetListResponse<GetListTenantListItemDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public GetListTenantQueryHandler(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListTenantListItemDto>> Handle(GetListTenantQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Tenant> tenants = await _tenantRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListTenantListItemDto> response = _mapper.Map<GetListResponse<GetListTenantListItemDto>>(tenants);
            return response;
        }
    }
}