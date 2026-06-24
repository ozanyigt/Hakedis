using Application.Features.Users.Constants;
using Application.Services.FirmRoles;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.Users.Queries.GetFirmRoles;

public class GetFirmRolesQuery : IRequest<IReadOnlyList<FirmRoleListItemDto>>, ISecuredRequest
{
    public string[] Roles => [UsersOperationClaims.Read];

    public class GetFirmRolesQueryHandler : IRequestHandler<GetFirmRolesQuery, IReadOnlyList<FirmRoleListItemDto>>
    {
        public Task<IReadOnlyList<FirmRoleListItemDto>> Handle(
            GetFirmRolesQuery request,
            CancellationToken cancellationToken
        )
        {
            IReadOnlyList<FirmRoleListItemDto> roles = Enum.GetValues<FirmRole>()
                .Select(role => new FirmRoleListItemDto
                {
                    Value = role,
                    Label = FirmRoleClaimMapper.GetDisplayName(role),
                    Description = FirmRoleClaimMapper.GetDescription(role),
                })
                .ToList();

            return Task.FromResult(roles);
        }
    }
}

public class FirmRoleListItemDto
{
    public FirmRole Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
