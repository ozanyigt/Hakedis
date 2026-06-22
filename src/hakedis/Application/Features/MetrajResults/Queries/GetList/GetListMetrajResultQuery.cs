using Application.Features.MetrajResults.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Queries.GetList;

public class GetListMetrajResultQuery : IRequest<GetListResponse<GetListMetrajResultListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListMetrajResultQueryHandler : IRequestHandler<GetListMetrajResultQuery, GetListResponse<GetListMetrajResultListItemDto>>
    {
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly IMapper _mapper;

        public GetListMetrajResultQueryHandler(IMetrajResultRepository metrajResultRepository, IMapper mapper)
        {
            _metrajResultRepository = metrajResultRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListMetrajResultListItemDto>> Handle(GetListMetrajResultQuery request, CancellationToken cancellationToken)
        {
            IPaginate<MetrajResult> metrajResults = await _metrajResultRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListMetrajResultListItemDto> response = _mapper.Map<GetListResponse<GetListMetrajResultListItemDto>>(metrajResults);
            return response;
        }
    }
}