using Application.Features.Sites.Constants;
using Application.Features.Sites.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.Sites.Constants.SitesOperationClaims;

namespace Application.Features.Sites.Commands.Update;

public class UpdateSiteCommand : IRequest<UpdatedSiteResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public required SiteStatus Status { get; set; }
    public string? Description { get; set; }

    public string[] Roles => [Admin, Write, SitesOperationClaims.Update];

    public class UpdateSiteCommandHandler : IRequestHandler<UpdateSiteCommand, UpdatedSiteResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISiteRepository _siteRepository;
        private readonly SiteBusinessRules _siteBusinessRules;

        public UpdateSiteCommandHandler(IMapper mapper, ISiteRepository siteRepository,
                                         SiteBusinessRules siteBusinessRules)
        {
            _mapper = mapper;
            _siteRepository = siteRepository;
            _siteBusinessRules = siteBusinessRules;
        }

        public async Task<UpdatedSiteResponse> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
        {
            Site? site = await _siteRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            await _siteBusinessRules.SiteShouldExistWhenSelected(site);
            site = _mapper.Map(request, site);

            await _siteRepository.UpdateAsync(site!);

            UpdatedSiteResponse response = _mapper.Map<UpdatedSiteResponse>(site);
            return response;
        }
    }
}