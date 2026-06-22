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

namespace Application.Features.Workers.Commands.Update;

public class UpdateWorkerCommand : IRequest<UpdatedWorkerResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string FullName { get; set; }
    public string? Trade { get; set; }
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public required bool IsActive { get; set; }

    public string[] Roles => [Admin, Write, WorkersOperationClaims.Update];

    public class UpdateWorkerCommandHandler : IRequestHandler<UpdateWorkerCommand, UpdatedWorkerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly WorkerBusinessRules _workerBusinessRules;

        public UpdateWorkerCommandHandler(IMapper mapper, IWorkerRepository workerRepository,
                                         WorkerBusinessRules workerBusinessRules)
        {
            _mapper = mapper;
            _workerRepository = workerRepository;
            _workerBusinessRules = workerBusinessRules;
        }

        public async Task<UpdatedWorkerResponse> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
        {
            Worker? worker = await _workerRepository.GetAsync(predicate: w => w.Id == request.Id, cancellationToken: cancellationToken);
            await _workerBusinessRules.WorkerShouldExistWhenSelected(worker);
            worker = _mapper.Map(request, worker);

            await _workerRepository.UpdateAsync(worker!);

            UpdatedWorkerResponse response = _mapper.Map<UpdatedWorkerResponse>(worker);
            return response;
        }
    }
}