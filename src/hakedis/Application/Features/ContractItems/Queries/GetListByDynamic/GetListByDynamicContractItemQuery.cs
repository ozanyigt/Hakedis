using Application.Features.ContractItems.Constants;
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
using static Application.Features.ContractItems.Constants.ContractItemsOperationClaims;

namespace Application.Features.ContractItems.Queries.GetListByDynamic;

public class GetListByDynamicContractItemQuery : IRequest<GetListResponse<GetListByDynamicContractItemListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicContractItemQueryHandler : IRequestHandler<GetListByDynamicContractItemQuery, GetListResponse<GetListByDynamicContractItemListItemDto>>
    {
        private readonly IContractItemRepository _contractItemRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicContractItemQueryHandler(IContractItemRepository contractItemRepository, IMapper mapper)
        {
            _contractItemRepository = contractItemRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicContractItemListItemDto>> Handle(GetListByDynamicContractItemQuery request, CancellationToken cancellationToken)
        {
            IPaginate<ContractItem> contractItems = await _contractItemRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicContractItemListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicContractItemListItemDto>>(contractItems);
            return response;
        }
    }
}
