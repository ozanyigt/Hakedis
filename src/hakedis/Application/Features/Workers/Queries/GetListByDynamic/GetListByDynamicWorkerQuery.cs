using Application.Features.Workers.Constants;
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
using static Application.Features.Workers.Constants.WorkersOperationClaims;

namespace Application.Features.Workers.Queries.GetListByDynamic;

public class GetListByDynamicWorkerQuery : IRequest<GetListResponse<GetListByDynamicWorkerListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicWorkerQueryHandler : IRequestHandler<GetListByDynamicWorkerQuery, GetListResponse<GetListByDynamicWorkerListItemDto>>
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicWorkerQueryHandler(IWorkerRepository workerRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicWorkerListItemDto>> Handle(GetListByDynamicWorkerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Worker> workers = await _workerRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicWorkerListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicWorkerListItemDto>>(workers);
            return response;
        }
    }
}
