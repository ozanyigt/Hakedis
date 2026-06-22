using Application.Features.HakedisPeriods.Constants;
using Application.Features.HakedisPeriods.Constants;
using Application.Features.HakedisPeriods.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisPeriods.Commands.Delete;

public class DeleteHakedisPeriodCommand : IRequest<DeletedHakedisPeriodResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, HakedisPeriodsOperationClaims.Delete];

    public class DeleteHakedisPeriodCommandHandler : IRequestHandler<DeleteHakedisPeriodCommand, DeletedHakedisPeriodResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
        private readonly HakedisPeriodBusinessRules _hakedisPeriodBusinessRules;

        public DeleteHakedisPeriodCommandHandler(IMapper mapper, IHakedisPeriodRepository hakedisPeriodRepository,
                                         HakedisPeriodBusinessRules hakedisPeriodBusinessRules)
        {
            _mapper = mapper;
            _hakedisPeriodRepository = hakedisPeriodRepository;
            _hakedisPeriodBusinessRules = hakedisPeriodBusinessRules;
        }

        public async Task<DeletedHakedisPeriodResponse> Handle(DeleteHakedisPeriodCommand request, CancellationToken cancellationToken)
        {
            HakedisPeriod? hakedisPeriod = await _hakedisPeriodRepository.GetAsync(predicate: hp => hp.Id == request.Id, cancellationToken: cancellationToken);
            await _hakedisPeriodBusinessRules.HakedisPeriodShouldExistWhenSelected(hakedisPeriod);

            await _hakedisPeriodRepository.DeleteAsync(hakedisPeriod!);

            DeletedHakedisPeriodResponse response = _mapper.Map<DeletedHakedisPeriodResponse>(hakedisPeriod);
            return response;
        }
    }
}