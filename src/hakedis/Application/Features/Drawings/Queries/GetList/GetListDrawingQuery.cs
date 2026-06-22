using Application.Features.Drawings.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Queries.GetList;

public class GetListDrawingQuery : IRequest<GetListResponse<GetListDrawingListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListDrawingQueryHandler : IRequestHandler<GetListDrawingQuery, GetListResponse<GetListDrawingListItemDto>>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMapper _mapper;

        public GetListDrawingQueryHandler(IDrawingRepository drawingRepository, IMapper mapper)
        {
            _drawingRepository = drawingRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListDrawingListItemDto>> Handle(GetListDrawingQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Drawing> drawings = await _drawingRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListDrawingListItemDto> response = _mapper.Map<GetListResponse<GetListDrawingListItemDto>>(drawings);
            return response;
        }
    }
}