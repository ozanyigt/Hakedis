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

namespace Application.Features.MetrajRuleTemplates.Commands.Create;

public class CreateMetrajRuleTemplateCommand : IRequest<CreatedMetrajRuleTemplateResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
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

    public string[] Roles => [Admin, Write, MetrajRuleTemplatesOperationClaims.Create];

    public class CreateMetrajRuleTemplateCommandHandler : IRequestHandler<CreateMetrajRuleTemplateCommand, CreatedMetrajRuleTemplateResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajRuleTemplateRepository _metrajRuleTemplateRepository;
        private readonly MetrajRuleTemplateBusinessRules _metrajRuleTemplateBusinessRules;

        public CreateMetrajRuleTemplateCommandHandler(IMapper mapper, IMetrajRuleTemplateRepository metrajRuleTemplateRepository,
                                         MetrajRuleTemplateBusinessRules metrajRuleTemplateBusinessRules)
        {
            _mapper = mapper;
            _metrajRuleTemplateRepository = metrajRuleTemplateRepository;
            _metrajRuleTemplateBusinessRules = metrajRuleTemplateBusinessRules;
        }

        public async Task<CreatedMetrajRuleTemplateResponse> Handle(CreateMetrajRuleTemplateCommand request, CancellationToken cancellationToken)
        {
            MetrajRuleTemplate metrajRuleTemplate = _mapper.Map<MetrajRuleTemplate>(request);

            await _metrajRuleTemplateRepository.AddAsync(metrajRuleTemplate);

            CreatedMetrajRuleTemplateResponse response = _mapper.Map<CreatedMetrajRuleTemplateResponse>(metrajRuleTemplate);
            return response;
        }
    }
}