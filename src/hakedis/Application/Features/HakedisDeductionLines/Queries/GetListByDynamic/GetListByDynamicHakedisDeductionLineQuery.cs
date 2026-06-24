using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.HakedisPeriods.Constants.HakedisPeriodsOperationClaims;

namespace Application.Features.HakedisDeductionLines.Queries.GetListByDynamic;

public class GetListByDynamicHakedisDeductionLineQuery
    : IRequest<GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto>>,
        ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = null!;
    public DynamicQuery Dynamic { get; set; } = null!;

    public string[] Roles => [Admin, Read];

    public class GetListByDynamicHakedisDeductionLineQueryHandler
        : IRequestHandler<
            GetListByDynamicHakedisDeductionLineQuery,
            GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto>
        >
    {
        private readonly IHakedisDeductionLineRepository _repository;
        private readonly IMapper _mapper;

        public GetListByDynamicHakedisDeductionLineQueryHandler(
            IHakedisDeductionLineRepository repository,
            IMapper mapper
        )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto>> Handle(
            GetListByDynamicHakedisDeductionLineQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<HakedisDeductionLine> lines = await _repository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            return _mapper.Map<GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto>>(lines);
        }
    }
}

public class GetListByDynamicHakedisDeductionLineListItemDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid HakedisPeriodId { get; set; }
    public DeductionCategory Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
