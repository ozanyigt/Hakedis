using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Security.Constants;

namespace Application.Features.DemoRequests.Commands.UpdateStatus;

public class UpdateDemoRequestStatusCommand : IRequest<UpdatedDemoRequestStatusResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public DemoRequestStatus Status { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin];

    public class UpdateDemoRequestStatusCommandHandler
        : IRequestHandler<UpdateDemoRequestStatusCommand, UpdatedDemoRequestStatusResponse>
    {
        private readonly IDemoRequestRepository _demoRequestRepository;

        public UpdateDemoRequestStatusCommandHandler(IDemoRequestRepository demoRequestRepository)
        {
            _demoRequestRepository = demoRequestRepository;
        }

        public async Task<UpdatedDemoRequestStatusResponse> Handle(
            UpdateDemoRequestStatusCommand request,
            CancellationToken cancellationToken
        )
        {
            DemoRequest? demoRequest = await _demoRequestRepository.GetAsync(
                predicate: item => item.Id == request.Id,
                cancellationToken: cancellationToken
            );

            if (demoRequest == null)
            {
                throw new BusinessException("Demo talebi bulunamadı.");
            }

            demoRequest.Status = request.Status;
            await _demoRequestRepository.UpdateAsync(demoRequest);

            return new UpdatedDemoRequestStatusResponse { Id = demoRequest.Id, Status = demoRequest.Status };
        }
    }
}

public class UpdatedDemoRequestStatusResponse
{
    public Guid Id { get; set; }
    public DemoRequestStatus Status { get; set; }
}
