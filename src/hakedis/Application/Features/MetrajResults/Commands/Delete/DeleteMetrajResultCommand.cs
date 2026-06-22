using Application.Features.MetrajResults.Constants;
using Application.Features.MetrajResults.Constants;
using Application.Features.MetrajResults.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Commands.Delete;

public class DeleteMetrajResultCommand : IRequest<DeletedMetrajResultResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, MetrajResultsOperationClaims.Delete];

    public class DeleteMetrajResultCommandHandler : IRequestHandler<DeleteMetrajResultCommand, DeletedMetrajResultResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly MetrajResultBusinessRules _metrajResultBusinessRules;

        public DeleteMetrajResultCommandHandler(IMapper mapper, IMetrajResultRepository metrajResultRepository,
                                         MetrajResultBusinessRules metrajResultBusinessRules)
        {
            _mapper = mapper;
            _metrajResultRepository = metrajResultRepository;
            _metrajResultBusinessRules = metrajResultBusinessRules;
        }

        public async Task<DeletedMetrajResultResponse> Handle(DeleteMetrajResultCommand request, CancellationToken cancellationToken)
        {
            MetrajResult? metrajResult = await _metrajResultRepository.GetAsync(predicate: mr => mr.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajResultBusinessRules.MetrajResultShouldExistWhenSelected(metrajResult);

            await _metrajResultRepository.DeleteAsync(metrajResult!);

            DeletedMetrajResultResponse response = _mapper.Map<DeletedMetrajResultResponse>(metrajResult);
            return response;
        }
    }
}