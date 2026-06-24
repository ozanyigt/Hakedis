using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.FirmRoles;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Security.Hashing;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommand : IRequest<CreatedUserResponse>, ISecuredRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid? TenantId { get; set; }
    public FirmRole FirmRole { get; set; }
    public FirmRole? SecondaryFirmRole { get; set; }

    public CreateUserCommand()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
    }

    public string[] Roles => [Admin, Write, UsersOperationClaims.Create];

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IFirmRoleAssignmentService _firmRoleAssignmentService;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules,
            IFirmRoleAssignmentService firmRoleAssignmentService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _firmRoleAssignmentService = firmRoleAssignmentService;
        }

        public async Task<CreatedUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(request.Email);
            await _userBusinessRules.TenantAndFirmRoleShouldBeValidForCreate(request.TenantId, request.FirmRole);

            Guid tenantId = await _userBusinessRules.ResolveTenantIdForCreateAsync(request.TenantId);

            User user = _mapper.Map<User>(request);
            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email.Trim();
            user.TenantId = tenantId;
            user.FirmRole = request.FirmRole;
            user.SecondaryFirmRole = request.SecondaryFirmRole;

            HashingHelper.CreatePasswordHash(
                request.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            User createdUser = await _userRepository.AddAsync(user);
            await _firmRoleAssignmentService.AssignAsync(
                createdUser.Id,
                request.FirmRole,
                request.SecondaryFirmRole,
                cancellationToken
            );

            CreatedUserResponse response = _mapper.Map<CreatedUserResponse>(createdUser);
            response.FirmRole = request.FirmRole;
            response.SecondaryFirmRole = request.SecondaryFirmRole;
            response.TenantId = tenantId;
            return response;
        }
    }
}
