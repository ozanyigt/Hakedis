using Application.Features.ContractItems.Constants;
using Application.Features.ContractItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.ContractItems.Constants.ContractItemsOperationClaims;

namespace Application.Features.ContractItems.Commands.Create;

public class CreateContractItemCommand : IRequest<CreatedContractItemResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public required MetrajKalemType KalemType { get; set; }
    public required string Description { get; set; }
    public required MeasurementUnit Unit { get; set; }
    public required decimal UnitPrice { get; set; }
    public decimal? ContractQuantity { get; set; }
    public required int SortOrder { get; set; }

    public string[] Roles => [Admin, Write, ContractItemsOperationClaims.Create];

    public class CreateContractItemCommandHandler : IRequestHandler<CreateContractItemCommand, CreatedContractItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IContractItemRepository _contractItemRepository;
        private readonly ContractItemBusinessRules _contractItemBusinessRules;

        public CreateContractItemCommandHandler(IMapper mapper, IContractItemRepository contractItemRepository,
                                         ContractItemBusinessRules contractItemBusinessRules)
        {
            _mapper = mapper;
            _contractItemRepository = contractItemRepository;
            _contractItemBusinessRules = contractItemBusinessRules;
        }

        public async Task<CreatedContractItemResponse> Handle(CreateContractItemCommand request, CancellationToken cancellationToken)
        {
            ContractItem contractItem = _mapper.Map<ContractItem>(request);

            await _contractItemRepository.AddAsync(contractItem);

            CreatedContractItemResponse response = _mapper.Map<CreatedContractItemResponse>(contractItem);
            return response;
        }
    }
}