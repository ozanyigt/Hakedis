using Application.Features.Workers.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Workers.Constants.WorkersOperationClaims;

namespace Application.Features.Workers.Queries.GetList;

public class GetListWorkerQuery : IRequest<GetListResponse<GetListWorkerListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListWorkerQueryHandler : IRequestHandler<GetListWorkerQuery, GetListResponse<GetListWorkerListItemDto>>
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public GetListWorkerQueryHandler(IWorkerRepository workerRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListWorkerListItemDto>> Handle(GetListWorkerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Worker> workers = await _workerRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListWorkerListItemDto> response = _mapper.Map<GetListResponse<GetListWorkerListItemDto>>(workers);
            return response;
        }
    }
}