using Application.Features.Tenants.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Pipelines.Authorization;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.Tenants.Constants.TenantsOperationClaims;

namespace Application.Features.Tenants.Queries.GetListByDynamic;

public class GetListByDynamicTenantQuery : IRequest<GetListResponse<GetListByDynamicTenantListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicTenantQueryHandler : IRequestHandler<GetListByDynamicTenantQuery, GetListResponse<GetListByDynamicTenantListItemDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicTenantQueryHandler(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicTenantListItemDto>> Handle(GetListByDynamicTenantQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Tenant> tenants = await _tenantRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicTenantListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicTenantListItemDto>>(tenants);
            return response;
        }
    }
}
