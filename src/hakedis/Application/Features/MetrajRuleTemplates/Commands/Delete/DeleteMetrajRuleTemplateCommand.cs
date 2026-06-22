using Application.Features.MetrajRuleTemplates.Constants;
using Application.Features.MetrajRuleTemplates.Constants;
using Application.Features.MetrajRuleTemplates.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.MetrajRuleTemplates.Constants.MetrajRuleTemplatesOperationClaims;

namespace Application.Features.MetrajRuleTemplates.Commands.Delete;

public class DeleteMetrajRuleTemplateCommand : IRequest<DeletedMetrajRuleTemplateResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, MetrajRuleTemplatesOperationClaims.Delete];

    public class DeleteMetrajRuleTemplateCommandHandler : IRequestHandler<DeleteMetrajRuleTemplateCommand, DeletedMetrajRuleTemplateResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly MetrajRuleTemplateBusinessRules _metrajRuleTemplateBusinessRules;

        public DeleteMetrajRuleTemplateCommandHandler(IMapper mapper, IMetrajRuleTemplateRepository metrajRuleTemplateRepository,
                                         MetrajRuleTemplateBusinessRules metrajRuleTemplateBusinessRules)
        {
            _mapper = mapper;
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _metrajRuleTemplateBusinessRules = metrajRuleTemplateBusinessRules;
        }

        public async Task<DeletedMetrajRuleTemplateResponse> Handle(DeleteMetrajRuleTemplateCommand request, CancellationToken cancellationToken)
        {
            MetrajRuleTemplate? metrajRuleTemplate = await _metrajRuleTemplateRepository.GetAsync(predicate: mrt => mrt.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajRuleTemplateBusinessRules.MetrajRuleTemplateShouldExistWhenSelected(metrajRuleTemplate);

            await _metrajRuleTemplateRepository.DeleteAsync(metrajRuleTemplate!);

            DeletedMetrajRuleTemplateResponse response = _mapper.Map<DeletedMetrajRuleTemplateResponse>(metrajRuleTemplate);
            return response;
        }
    }
}