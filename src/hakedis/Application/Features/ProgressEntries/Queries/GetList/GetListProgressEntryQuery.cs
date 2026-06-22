using Application.Features.ProgressEntries.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.ProgressEntries.Constants.ProgressEntriesOperationClaims;

namespace Application.Features.ProgressEntries.Queries.GetList;

public class GetListProgressEntryQuery : IRequest<GetListResponse<GetListProgressEntryListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListProgressEntryQueryHandler : IRequestHandler<GetListProgressEntryQuery, GetListResponse<GetListProgressEntryListItemDto>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly IMapper _mapper;

        public GetListProgressEntryQueryHandler(IProgressEntryRepository progressEntryRepository, IMapper mapper)
        {
            _progressEntryRepository = progressEntryRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListProgressEntryListItemDto>> Handle(GetListProgressEntryQuery request, CancellationToken cancellationToken)
        {
            IPaginate<ProgressEntry> progressEntries = await _progressEntryRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListProgressEntryListItemDto> response = _mapper.Map<GetListResponse<GetListProgressEntryListItemDto>>(progressEntries);
            return response;
        }
    }
}