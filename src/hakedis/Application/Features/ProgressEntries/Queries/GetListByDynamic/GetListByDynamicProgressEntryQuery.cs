using Application.Features.ProgressEntries.Constants;
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
using static Application.Features.ProgressEntries.Constants.ProgressEntriesOperationClaims;

namespace Application.Features.ProgressEntries.Queries.GetListByDynamic;

public class GetListByDynamicProgressEntryQuery : IRequest<GetListResponse<GetListByDynamicProgressEntryListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicProgressEntryQueryHandler : IRequestHandler<GetListByDynamicProgressEntryQuery, GetListResponse<GetListByDynamicProgressEntryListItemDto>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicProgressEntryQueryHandler(IProgressEntryRepository progressEntryRepository, IMapper mapper)
        {
            _progressEntryRepository = progressEntryRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicProgressEntryListItemDto>> Handle(GetListByDynamicProgressEntryQuery request, CancellationToken cancellationToken)
        {
            IPaginate<ProgressEntry> progressEntries = await _progressEntryRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicProgressEntryListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicProgressEntryListItemDto>>(progressEntries);
            return response;
        }
    }
}
