using Application.Features.Sites.Constants;
using Application.Features.Sites.Constants;
using Application.Features.Sites.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Sites.Constants.SitesOperationClaims;

namespace Application.Features.Sites.Commands.Delete;

public class DeleteSiteCommand : IRequest<DeletedSiteResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, SitesOperationClaims.Delete];

    public class DeleteSiteCommandHandler : IRequestHandler<DeleteSiteCommand, DeletedSiteResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISiteRepository _siteRepository;
        private readonly SiteBusinessRules _siteBusinessRules;

        public DeleteSiteCommandHandler(IMapper mapper, ISiteRepository siteRepository,
                                         SiteBusinessRules siteBusinessRules)
        {
            _mapper = mapper;
            _siteRepository = siteRepository;
            _siteBusinessRules = siteBusinessRules;
        }

        public async Task<DeletedSiteResponse> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
        {
            Site? site = await _siteRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            await _siteBusinessRules.SiteShouldExistWhenSelected(site);

            await _siteRepository.DeleteAsync(site!);

            DeletedSiteResponse response = _mapper.Map<DeletedSiteResponse>(site);
            return response;
        }
    }
}