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

namespace Application.Features.Workers.Commands.Create;

public class CreateWorkerCommand : IRequest<CreatedWorkerResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid TenantId { get; set; }
    public required string FullName { get; set; }
    public string? Trade { get; set; }
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public required bool IsActive { get; set; }

    public string[] Roles => [Admin, Write, WorkersOperationClaims.Create];

    public class CreateWorkerCommandHandler : IRequestHandler<CreateWorkerCommand, CreatedWorkerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly WorkerBusinessRules _workerBusinessRules;

        public CreateWorkerCommandHandler(IMapper mapper, IWorkerRepository workerRepository,
                                         WorkerBusinessRules workerBusinessRules)
        {
            _mapper = mapper;
            _workerRepository = workerRepository;
            _workerBusinessRules = workerBusinessRules;
        }

        public async Task<CreatedWorkerResponse> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
        {
            Worker worker = _mapper.Map<Worker>(request);

            await _workerRepository.AddAsync(worker);

            CreatedWorkerResponse response = _mapper.Map<CreatedWorkerResponse>(worker);
            return response;
        }
    }
}