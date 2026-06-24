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

namespace Application.Features.ContractItems.Commands.Update;

public class UpdateContractItemCommand : IRequest<UpdatedContractItemResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ProjectId { get; set; }
    public required MetrajKalemType KalemType { get; set; }
    public required string Description { get; set; }
    public required MeasurementUnit Unit { get; set; }
    public required decimal UnitPrice { get; set; }
    public decimal? ContractQuantity { get; set; }
    public required int SortOrder { get; set; }

    public string[] Roles => [Admin, Write, ContractItemsOperationClaims.Update];

    public class UpdateContractItemCommandHandler : IRequestHandler<UpdateContractItemCommand, UpdatedContractItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IContractItemRepository _contractItemRepository;
        private readonly ContractItemBusinessRules _contractItemBusinessRules;

        public UpdateContractItemCommandHandler(IMapper mapper, IContractItemRepository contractItemRepository,
                                         ContractItemBusinessRules contractItemBusinessRules)
        {
            _mapper = mapper;
            _contractItemRepository = contractItemRepository;
            _contractItemBusinessRules = contractItemBusinessRules;
        }

        public async Task<UpdatedContractItemResponse> Handle(UpdateContractItemCommand request, CancellationToken cancellationToken)
        {
            ContractItem? contractItem = await _contractItemRepository.GetAsync(predicate: ci => ci.Id == request.Id, cancellationToken: cancellationToken);
            await _contractItemBusinessRules.ContractItemShouldExistWhenSelected(contractItem);
            contractItem = _mapper.Map(request, contractItem);

            await _contractItemRepository.UpdateAsync(contractItem!);

            UpdatedContractItemResponse response = _mapper.Map<UpdatedContractItemResponse>(contractItem);
            return response;
        }
    }
}