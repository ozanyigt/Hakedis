using Application.Features.ContractItems.Constants;
using Application.Features.ContractItems.Constants;
using Application.Features.ContractItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.ContractItems.Constants.ContractItemsOperationClaims;

namespace Application.Features.ContractItems.Commands.Delete;

public class DeleteContractItemCommand : IRequest<DeletedContractItemResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, ContractItemsOperationClaims.Delete];

    public class DeleteContractItemCommandHandler : IRequestHandler<DeleteContractItemCommand, DeletedContractItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IContractItemRepository _contractItemRepository;
        private readonly ContractItemBusinessRules _contractItemBusinessRules;

        public DeleteContractItemCommandHandler(IMapper mapper, IContractItemRepository contractItemRepository,
                                         ContractItemBusinessRules contractItemBusinessRules)
        {
            _mapper = mapper;
            _contractItemRepository = contractItemRepository;
            _contractItemBusinessRules = contractItemBusinessRules;
        }

        public async Task<DeletedContractItemResponse> Handle(DeleteContractItemCommand request, CancellationToken cancellationToken)
        {
            ContractItem? contractItem = await _contractItemRepository.GetAsync(predicate: ci => ci.Id == request.Id, cancellationToken: cancellationToken);
            await _contractItemBusinessRules.ContractItemShouldExistWhenSelected(contractItem);

            await _contractItemRepository.DeleteAsync(contractItem!);

            DeletedContractItemResponse response = _mapper.Map<DeletedContractItemResponse>(contractItem);
            return response;
        }
    }
}