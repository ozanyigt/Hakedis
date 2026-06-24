using Application.Features.MetrajRuleTemplates.Constants;
using Application.Features.MetrajRuleTemplates.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.MetrajRuleTemplates.Constants.MetrajRuleTemplatesOperationClaims;

namespace Application.Features.MetrajRuleTemplates.Commands.Update;

public class UpdateMetrajRuleTemplateCommand : IRequest<UpdatedMetrajRuleTemplateResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required MetrajKalemType KalemType { get; set; }
    public required string LayerPatterns { get; set; }
    public required string EntityTypes { get; set; }
    public required MeasurementUnit Unit { get; set; }
    public decimal? DefaultThickness { get; set; }
    public decimal? DefaultHeight { get; set; }
    public required bool DeductOpenings { get; set; }
    public string? OpeningLayerPatterns { get; set; }
    public required bool IsDefault { get; set; }
    public required bool IsActive { get; set; }

    public string[] Roles => [Admin, Write, MetrajRuleTemplatesOperationClaims.Update];

    public class UpdateMetrajRuleTemplateCommandHandler : IRequestHandler<UpdateMetrajRuleTemplateCommand, UpdatedMetrajRuleTemplateResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly MetrajRuleTemplateBusinessRules _metrajRuleTemplateBusinessRules;

        public UpdateMetrajRuleTemplateCommandHandler(IMapper mapper, IMetrajRuleTemplateRepository metrajRuleTemplateRepository,
                                         MetrajRuleTemplateBusinessRules metrajRuleTemplateBusinessRules)
        {
            _mapper = mapper;
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _metrajRuleTemplateBusinessRules = metrajRuleTemplateBusinessRules;
        }

        public async Task<UpdatedMetrajRuleTemplateResponse> Handle(UpdateMetrajRuleTemplateCommand request, CancellationToken cancellationToken)
        {
            MetrajRuleTemplate? metrajRuleTemplate = await _metrajRuleTemplateRepository.GetAsync(predicate: mrt => mrt.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajRuleTemplateBusinessRules.MetrajRuleTemplateShouldExistWhenSelected(metrajRuleTemplate);
            metrajRuleTemplate = _mapper.Map(request, metrajRuleTemplate);

            await _metrajRuleTemplateRepository.UpdateAsync(metrajRuleTemplate!);

            UpdatedMetrajRuleTemplateResponse response = _mapper.Map<UpdatedMetrajRuleTemplateResponse>(metrajRuleTemplate);
            return response;
        }
    }
}