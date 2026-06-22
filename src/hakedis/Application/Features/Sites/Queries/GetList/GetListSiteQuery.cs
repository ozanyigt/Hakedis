using Application.Features.Sites.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Sites.Constants.SitesOperationClaims;

namespace Application.Features.Sites.Queries.GetList;

public class GetListSiteQuery : IRequest<GetListResponse<GetListSiteListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListSiteQueryHandler : IRequestHandler<GetListSiteQuery, GetListResponse<GetListSiteListItemDto>>
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;

        public GetListSiteQueryHandler(ISiteRepository siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListSiteListItemDto>> Handle(GetListSiteQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Site> sites = await _siteRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListSiteListItemDto> response = _mapper.Map<GetListResponse<GetListSiteListItemDto>>(sites);
            return response;
        }
    }
}