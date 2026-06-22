using Application.Features.ContractItems.Constants;
using Application.Features.ContractItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ContractItems.Constants.ContractItemsOperationClaims;

namespace Application.Features.ContractItems.Queries.GetById;

public class GetByIdContractItemQuery : IRequest<GetByIdContractItemResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdContractItemQueryHandler : IRequestHandler<GetByIdContractItemQuery, GetByIdContractItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IContractItemRepository _contractItemRepository;
        private readonly ContractItemBusinessRules _contractItemBusinessRules;

        public GetByIdContractItemQueryHandler(IMapper mapper, IContractItemRepository contractItemRepository, ContractItemBusinessRules contractItemBusinessRules)
        {
            _mapper = mapper;
            _contractItemRepository = contractItemRepository;
            _contractItemBusinessRules = contractItemBusinessRules;
        }

        public async Task<GetByIdContractItemResponse> Handle(GetByIdContractItemQuery request, CancellationToken cancellationToken)
        {
            ContractItem? contractItem = await _contractItemRepository.GetAsync(predicate: ci => ci.Id == request.Id, cancellationToken: cancellationToken);
            await _contractItemBusinessRules.ContractItemShouldExistWhenSelected(contractItem);

            GetByIdContractItemResponse response = _mapper.Map<GetByIdContractItemResponse>(contractItem);
            return response;
        }
    }
}