using Application.Features.MetrajResults.Constants;
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
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Queries.GetListByDynamic;

public class GetListByDynamicMetrajResultQuery : IRequest<GetListResponse<GetListByDynamicMetrajResultListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicMetrajResultQueryHandler : IRequestHandler<GetListByDynamicMetrajResultQuery, GetListResponse<GetListByDynamicMetrajResultListItemDto>>
    {
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicMetrajResultQueryHandler(IMetrajResultRepository metrajResultRepository, IMapper mapper)
        {
            _metrajResultRepository = metrajResultRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicMetrajResultListItemDto>> Handle(GetListByDynamicMetrajResultQuery request, CancellationToken cancellationToken)
        {
            IPaginate<MetrajResult> metrajResults = await _metrajResultRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicMetrajResultListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicMetrajResultListItemDto>>(metrajResults);
            return response;
        }
    }
}
