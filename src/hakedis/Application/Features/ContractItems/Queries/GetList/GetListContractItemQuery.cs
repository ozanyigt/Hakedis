using Application.Features.ContractItems.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.ContractItems.Constants.ContractItemsOperationClaims;

namespace Application.Features.ContractItems.Queries.GetList;

public class GetListContractItemQuery : IRequest<GetListResponse<GetListContractItemListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListContractItemQueryHandler : IRequestHandler<GetListContractItemQuery, GetListResponse<GetListContractItemListItemDto>>
    {
        private readonly IContractItemRepository _contractItemRepository;
        private readonly IMapper _mapper;

        public GetListContractItemQueryHandler(IContractItemRepository contractItemRepository, IMapper mapper)
        {
            _contractItemRepository = contractItemRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListContractItemListItemDto>> Handle(GetListContractItemQuery request, CancellationToken cancellationToken)
        {
            IPaginate<ContractItem> contractItems = await _contractItemRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListContractItemListItemDto> response = _mapper.Map<GetListResponse<GetListContractItemListItemDto>>(contractItems);
            return response;
        }
    }
}