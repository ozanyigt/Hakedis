using Application.Features.MetrajRuleTemplates.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.MetrajRuleTemplates.Constants.MetrajRuleTemplatesOperationClaims;

namespace Application.Features.MetrajRuleTemplates.Queries.GetList;

public class GetListMetrajRuleTemplateQuery : IRequest<GetListResponse<GetListMetrajRuleTemplateListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListMetrajRuleTemplateQueryHandler : IRequestHandler<GetListMetrajRuleTemplateQuery, GetListResponse<GetListMetrajRuleTemplateListItemDto>>
    {
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly IMapper _mapper;

        public GetListMetrajRuleTemplateQueryHandler(IMetrajRuleTemplateRepository metrajRuleTemplateRepository, IMapper mapper)
        {
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListMetrajRuleTemplateListItemDto>> Handle(GetListMetrajRuleTemplateQuery request, CancellationToken cancellationToken)
        {
            IPaginate<MetrajRuleTemplate> metrajRuleTemplates = await _metrajRuleTemplateRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListMetrajRuleTemplateListItemDto> response = _mapper.Map<GetListResponse<GetListMetrajRuleTemplateListItemDto>>(metrajRuleTemplates);
            return response;
        }
    }
}