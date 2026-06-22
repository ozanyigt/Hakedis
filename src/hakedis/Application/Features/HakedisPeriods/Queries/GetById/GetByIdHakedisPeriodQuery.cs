using Application.Features.HakedisPeriods.Constants;
using Application.Features.HakedisPeriods.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisPeriods.Queries.GetById;

public class GetByIdHakedisPeriodQuery : IRequest<GetByIdHakedisPeriodResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdHakedisPeriodQueryHandler : IRequestHandler<GetByIdHakedisPeriodQuery, GetByIdHakedisPeriodResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHakedisPeriodRepository _hakedisPeriodRepository;
        private readonly HakedisPeriodBusinessRules _hakedisPeriodBusinessRules;

        public GetByIdHakedisPeriodQueryHandler(IMapper mapper, IHakedisPeriodRepository hakedisPeriodRepository, HakedisPeriodBusinessRules hakedisPeriodBusinessRules)
        {
            _mapper = mapper;
            _hakedisPeriodRepository = hakedisPeriodRepository;
            _hakedisPeriodBusinessRules = hakedisPeriodBusinessRules;
        }

        public async Task<GetByIdHakedisPeriodResponse> Handle(GetByIdHakedisPeriodQuery request, CancellationToken cancellationToken)
        {
            HakedisPeriod? hakedisPeriod = await _hakedisPeriodRepository.GetAsync(predicate: hp => hp.Id == request.Id, cancellationToken: cancellationToken);
            await _hakedisPeriodBusinessRules.HakedisPeriodShouldExistWhenSelected(hakedisPeriod);

            GetByIdHakedisPeriodResponse response = _mapper.Map<GetByIdHakedisPeriodResponse>(hakedisPeriod);
            return response;
        }
    }
}