using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using NArchitecture.Core.Security.Constants;

namespace Application.Features.DemoRequests.Queries.GetList;

public class GetListDemoRequestQuery : IRequest<GetListResponse<GetListDemoRequestListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = new();

    public string[] Roles => [GeneralOperationClaims.Admin];

    public class GetListDemoRequestQueryHandler
        : IRequestHandler<GetListDemoRequestQuery, GetListResponse<GetListDemoRequestListItemDto>>
    {
        private readonly IDemoRequestRepository _demoRequestRepository;

        public GetListDemoRequestQueryHandler(IDemoRequestRepository demoRequestRepository)
        {
            _demoRequestRepository = demoRequestRepository;
        }

        public async Task<GetListResponse<GetListDemoRequestListItemDto>> Handle(
            GetListDemoRequestQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<DemoRequest> demoRequests = await _demoRequestRepository.GetListAsync(
                orderBy: query => query.OrderByDescending(item => item.CreatedDate),
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            return new GetListResponse<GetListDemoRequestListItemDto>
            {
                Items = demoRequests
                    .Items.Select(item => new GetListDemoRequestListItemDto
                    {
                        Id = item.Id,
                        CompanyName = item.CompanyName,
                        ContactName = item.ContactName,
                        Email = item.Email,
                        Phone = item.Phone,
                        Interest = item.Interest,
                        Message = item.Message,
                        Status = item.Status,
                        CreatedDate = item.CreatedDate,
                    })
                    .ToList(),
                Index = demoRequests.Index,
                Size = demoRequests.Size,
                Count = demoRequests.Count,
                Pages = demoRequests.Pages,
                HasPrevious = demoRequests.HasPrevious,
                HasNext = demoRequests.HasNext,
            };
        }
    }
}

public class GetListDemoRequestListItemDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DemoRequestStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
}
