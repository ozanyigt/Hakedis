using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.FirmRoles;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.UpdateFirmRole;

public class UpdateUserFirmRoleCommand : IRequest<UpdatedUserFirmRoleResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public FirmRole FirmRole { get; set; }
    public FirmRole? SecondaryFirmRole { get; set; }

    public string[] Roles => [Admin, Write, UsersOperationClaims.Update];

    public class UpdateUserFirmRoleCommandHandler : IRequestHandler<UpdateUserFirmRoleCommand, UpdatedUserFirmRoleResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IFirmRoleAssignmentService _firmRoleAssignmentService;

        public UpdateUserFirmRoleCommandHandler(
            IUserRepository userRepository,
            UserBusinessRules userBusinessRules,
            IFirmRoleAssignmentService firmRoleAssignmentService
        )
        {
            _userRepository = userRepository;
            _userBusinessRules = userBusinessRules;
            _firmRoleAssignmentService = firmRoleAssignmentService;
        }

        public async Task<UpdatedUserFirmRoleResponse> Handle(
            UpdateUserFirmRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            User? user = await _userRepository.GetAsync(
                predicate: item => item.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _userBusinessRules.UserShouldBeManageableByCaller(user!);

            user!.FirmRole = request.FirmRole;
            user.SecondaryFirmRole = request.SecondaryFirmRole;
            await _userRepository.UpdateAsync(user);

            await _firmRoleAssignmentService.AssignAsync(
                user.Id,
                request.FirmRole,
                request.SecondaryFirmRole,
                cancellationToken
            );

            return new UpdatedUserFirmRoleResponse
            {
                Id = user.Id,
                FirmRole = request.FirmRole,
                SecondaryFirmRole = request.SecondaryFirmRole,
            };
        }
    }
}
