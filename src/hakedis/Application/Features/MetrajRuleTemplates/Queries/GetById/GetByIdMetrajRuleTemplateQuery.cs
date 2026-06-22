using Application.Features.MetrajRuleTemplates.Constants;
using Application.Features.MetrajRuleTemplates.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.MetrajRuleTemplates.Constants.MetrajRuleTemplatesOperationClaims;

namespace Application.Features.MetrajRuleTemplates.Queries.GetById;

public class GetByIdMetrajRuleTemplateQuery : IRequest<GetByIdMetrajRuleTemplateResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdMetrajRuleTemplateQueryHandler : IRequestHandler<GetByIdMetrajRuleTemplateQuery, GetByIdMetrajRuleTemplateResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly MetrajRuleTemplateBusinessRules _metrajRuleTemplateBusinessRules;

        public GetByIdMetrajRuleTemplateQueryHandler(IMapper mapper, IMetrajRuleTemplateRepository metrajRuleTemplateRepository, MetrajRuleTemplateBusinessRules metrajRuleTemplateBusinessRules)
        {
            _mapper = mapper;
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _metrajRuleTemplateBusinessRules = metrajRuleTemplateBusinessRules;
        }

        public async Task<GetByIdMetrajRuleTemplateResponse> Handle(GetByIdMetrajRuleTemplateQuery request, CancellationToken cancellationToken)
        {
            MetrajRuleTemplate? metrajRuleTemplate = await _metrajRuleTemplateRepository.GetAsync(predicate: mrt => mrt.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajRuleTemplateBusinessRules.MetrajRuleTemplateShouldExistWhenSelected(metrajRuleTemplate);

            GetByIdMetrajRuleTemplateResponse response = _mapper.Map<GetByIdMetrajRuleTemplateResponse>(metrajRuleTemplate);
            return response;
        }
    }
}