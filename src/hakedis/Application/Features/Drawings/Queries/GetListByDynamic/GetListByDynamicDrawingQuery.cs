using Application.Features.Drawings.Constants;
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
using static Application.Features.Drawings.Constants.DrawingsOperationClaims;

namespace Application.Features.Drawings.Queries.GetListByDynamic;

public class GetListByDynamicDrawingQuery : IRequest<GetListResponse<GetListByDynamicDrawingListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicDrawingQueryHandler : IRequestHandler<GetListByDynamicDrawingQuery, GetListResponse<GetListByDynamicDrawingListItemDto>>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicDrawingQueryHandler(IDrawingRepository drawingRepository, IMapper mapper)
        {
            _drawingRepository = drawingRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicDrawingListItemDto>> Handle(GetListByDynamicDrawingQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Drawing> drawings = await _drawingRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicDrawingListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicDrawingListItemDto>>(drawings);
            return response;
        }
    }
}
