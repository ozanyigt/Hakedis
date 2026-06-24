using Application.Features.Users.Constants;
using Application.Services.CurrentUser;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using System.Linq.Expressions;

namespace Application.Features.Users.Queries.GetList;

public class GetListUserQuery : IRequest<GetListResponse<GetListUserListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public Guid? TenantId { get; set; }

    public string[] Roles => [UsersOperationClaims.Read];

    public GetListUserQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public class GetListUserQueryHandler : IRequestHandler<GetListUserQuery, GetListResponse<GetListUserListItemDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetListUserQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetListResponse<GetListUserListItemDto>> Handle(
            GetListUserQuery request,
            CancellationToken cancellationToken
        )
        {
            Expression<Func<User, bool>>? predicate = null;

            if (_currentUserService.IsGlobalAdmin)
            {
                if (request.TenantId.HasValue)
                {
                    predicate = user => user.TenantId == request.TenantId;
                }
            }
            else
            {
                User? caller = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (caller?.TenantId != null)
                {
                    Guid tenantId = caller.TenantId.Value;
                    predicate = user => user.TenantId == tenantId;
                }
            }

            IPaginate<User> users = await _userRepository.GetListAsync(
                predicate: predicate,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            return _mapper.Map<GetListResponse<GetListUserListItemDto>>(users);
        }
    }
}
