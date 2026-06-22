using Application.Features.Workers.Constants;
using Application.Features.Workers.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Workers.Constants.WorkersOperationClaims;

namespace Application.Features.Workers.Queries.GetById;

public class GetByIdWorkerQuery : IRequest<GetByIdWorkerResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdWorkerQueryHandler : IRequestHandler<GetByIdWorkerQuery, GetByIdWorkerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly WorkerBusinessRules _workerBusinessRules;

        public GetByIdWorkerQueryHandler(IMapper mapper, IWorkerRepository workerRepository, WorkerBusinessRules workerBusinessRules)
        {
            _mapper = mapper;
            _workerRepository = workerRepository;
            _workerBusinessRules = workerBusinessRules;
        }

        public async Task<GetByIdWorkerResponse> Handle(GetByIdWorkerQuery request, CancellationToken cancellationToken)
        {
            Worker? worker = await _workerRepository.GetAsync(predicate: w => w.Id == request.Id, cancellationToken: cancellationToken);
            await _workerBusinessRules.WorkerShouldExistWhenSelected(worker);

            GetByIdWorkerResponse response = _mapper.Map<GetByIdWorkerResponse>(worker);
            return response;
        }
    }
}