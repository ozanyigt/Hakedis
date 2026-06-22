using Application.Features.Sites.Constants;
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
using static Application.Features.Sites.Constants.SitesOperationClaims;

namespace Application.Features.Sites.Queries.GetListByDynamic;

public class GetListByDynamicSiteQuery : IRequest<GetListResponse<GetListByDynamicSiteListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicSiteQueryHandler : IRequestHandler<GetListByDynamicSiteQuery, GetListResponse<GetListByDynamicSiteListItemDto>>
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicSiteQueryHandler(ISiteRepository siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicSiteListItemDto>> Handle(GetListByDynamicSiteQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Site> sites = await _siteRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicSiteListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicSiteListItemDto>>(sites);
            return response;
        }
    }
}
