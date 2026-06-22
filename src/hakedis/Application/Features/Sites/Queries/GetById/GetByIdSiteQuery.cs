using Application.Features.Sites.Constants;
using Application.Features.Sites.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Sites.Constants.SitesOperationClaims;

namespace Application.Features.Sites.Queries.GetById;

public class GetByIdSiteQuery : IRequest<GetByIdSiteResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdSiteQueryHandler : IRequestHandler<GetByIdSiteQuery, GetByIdSiteResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISiteRepository _siteRepository;
        private readonly SiteBusinessRules _siteBusinessRules;

        public GetByIdSiteQueryHandler(IMapper mapper, ISiteRepository siteRepository, SiteBusinessRules siteBusinessRules)
        {
            _mapper = mapper;
            _siteRepository = siteRepository;
            _siteBusinessRules = siteBusinessRules;
        }

        public async Task<GetByIdSiteResponse> Handle(GetByIdSiteQuery request, CancellationToken cancellationToken)
        {
            Site? site = await _siteRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            await _siteBusinessRules.SiteShouldExistWhenSelected(site);

            GetByIdSiteResponse response = _mapper.Map<GetByIdSiteResponse>(site);
            return response;
        }
    }
}