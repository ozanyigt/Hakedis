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

namespace Application.Features.Sites.Commands.Create;

public class CreateSiteCommand : IRequest<CreatedSiteResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public required SiteStatus Status { get; set; }
    public string? Description { get; set; }

    public string[] Roles => [Admin, Write, SitesOperationClaims.Create];

    public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, CreatedSiteResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISiteRepository _siteRepository;
        private readonly SiteBusinessRules _siteBusinessRules;

        public CreateSiteCommandHandler(IMapper mapper, ISiteRepository siteRepository,
                                         SiteBusinessRules siteBusinessRules)
        {
            _mapper = mapper;
            _siteRepository = siteRepository;
            _siteBusinessRules = siteBusinessRules;
        }

        public async Task<CreatedSiteResponse> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
        {
            Site site = _mapper.Map<Site>(request);

            await _siteRepository.AddAsync(site);

            CreatedSiteResponse response = _mapper.Map<CreatedSiteResponse>(site);
            return response;
        }
    }
}