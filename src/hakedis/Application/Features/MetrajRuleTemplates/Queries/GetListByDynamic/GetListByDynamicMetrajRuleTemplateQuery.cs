using Application.Features.MetrajRuleTemplates.Constants;
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
using static Application.Features.MetrajRuleTemplates.Constants.MetrajRuleTemplatesOperationClaims;

namespace Application.Features.MetrajRuleTemplates.Queries.GetListByDynamic;

public class GetListByDynamicMetrajRuleTemplateQuery : IRequest<GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicMetrajRuleTemplateQueryHandler : IRequestHandler<GetListByDynamicMetrajRuleTemplateQuery, GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto>>
    {
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicMetrajRuleTemplateQueryHandler(IMetrajRuleTemplateRepository metrajRuleTemplateRepository, IMapper mapper)
        {
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto>> Handle(GetListByDynamicMetrajRuleTemplateQuery request, CancellationToken cancellationToken)
        {
            IPaginate<MetrajRuleTemplate> metrajRuleTemplates = await _metrajRuleTemplateRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto>>(metrajRuleTemplates);
            return response;
        }
    }
}
