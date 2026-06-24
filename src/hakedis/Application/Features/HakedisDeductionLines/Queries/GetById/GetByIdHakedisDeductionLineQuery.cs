using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisDeductionLines.Queries.GetById;

public class GetByIdHakedisDeductionLineQuery : IRequest<GetByIdHakedisDeductionLineResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdHakedisDeductionLineQueryHandler
        : IRequestHandler<GetByIdHakedisDeductionLineQuery, GetByIdHakedisDeductionLineResponse>
    {
        private readonly IHakedisDeductionLineRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdHakedisDeductionLineQueryHandler(IHakedisDeductionLineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetByIdHakedisDeductionLineResponse> Handle(
            GetByIdHakedisDeductionLineQuery request,
            CancellationToken cancellationToken
        )
        {
            HakedisDeductionLine? line = await _repository.GetAsync(
                predicate: l => l.Id == request.Id,
                cancellationToken: cancellationToken
            );

            if (line is null)
                throw new InvalidOperationException("Kesinti satırı bulunamadı.");

            return _mapper.Map<GetByIdHakedisDeductionLineResponse>(line);
        }
    }
}

public class GetByIdHakedisDeductionLineResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public DeductionCategory Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
