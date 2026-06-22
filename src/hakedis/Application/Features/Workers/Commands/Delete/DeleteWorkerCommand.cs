using Application.Features.Workers.Constants;
using Application.Features.Workers.Constants;
using Application.Features.Workers.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Workers.Constants.WorkersOperationClaims;

namespace Application.Features.Workers.Commands.Delete;

public class DeleteWorkerCommand : IRequest<DeletedWorkerResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, WorkersOperationClaims.Delete];

    public class DeleteWorkerCommandHandler : IRequestHandler<DeleteWorkerCommand, DeletedWorkerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly WorkerBusinessRules _workerBusinessRules;

        public DeleteWorkerCommandHandler(IMapper mapper, IWorkerRepository workerRepository,
                                         WorkerBusinessRules workerBusinessRules)
        {
            _mapper = mapper;
            _workerRepository = workerRepository;
            _workerBusinessRules = workerBusinessRules;
        }

        public async Task<DeletedWorkerResponse> Handle(DeleteWorkerCommand request, CancellationToken cancellationToken)
        {
            Worker? worker = await _workerRepository.GetAsync(predicate: w => w.Id == request.Id, cancellationToken: cancellationToken);
            await _workerBusinessRules.WorkerShouldExistWhenSelected(worker);

            await _workerRepository.DeleteAsync(worker!);

            DeletedWorkerResponse response = _mapper.Map<DeletedWorkerResponse>(worker);
            return response;
        }
    }
}