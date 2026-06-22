using Application.Features.MetrajResults.Constants;
using Application.Features.MetrajResults.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.MetrajResults.Queries.GetById;

public class GetByIdMetrajResultQuery : IRequest<GetByIdMetrajResultResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdMetrajResultQueryHandler : IRequestHandler<GetByIdMetrajResultQuery, GetByIdMetrajResultResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMetrajResultRepository _metrajResultRepository;
        private readonly MetrajResultBusinessRules _metrajResultBusinessRules;

        public GetByIdMetrajResultQueryHandler(IMapper mapper, IMetrajResultRepository metrajResultRepository, MetrajResultBusinessRules metrajResultBusinessRules)
        {
            _mapper = mapper;
            _metrajResultRepository = metrajResultRepository;
            _metrajResultBusinessRules = metrajResultBusinessRules;
        }

        public async Task<GetByIdMetrajResultResponse> Handle(GetByIdMetrajResultQuery request, CancellationToken cancellationToken)
        {
            MetrajResult? metrajResult = await _metrajResultRepository.GetAsync(predicate: mr => mr.Id == request.Id, cancellationToken: cancellationToken);
            await _metrajResultBusinessRules.MetrajResultShouldExistWhenSelected(metrajResult);

            GetByIdMetrajResultResponse response = _mapper.Map<GetByIdMetrajResultResponse>(metrajResult);
            return response;
        }
    }
}