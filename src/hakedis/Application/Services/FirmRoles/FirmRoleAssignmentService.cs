using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.FirmRoles;

public class FirmRoleAssignmentService : IFirmRoleAssignmentService
{
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IOperationClaimRepository _operationClaimRepository;

    public FirmRoleAssignmentService(
        IUserOperationClaimRepository userOperationClaimRepository,
        IOperationClaimRepository operationClaimRepository
    )
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _operationClaimRepository = operationClaimRepository;
    }

    public async Task AssignAsync(
        Guid userId,
        FirmRole primaryRole,
        FirmRole? secondaryRole,
        CancellationToken cancellationToken = default
    )
    {
        IReadOnlyList<string> claimNames = FirmRoleClaimMapper.GetClaimNames(primaryRole, secondaryRole);

        IPaginate<UserOperationClaim> existingClaims = await _userOperationClaimRepository.GetListAsync(
            predicate: claim => claim.UserId == userId,
            cancellationToken: cancellationToken
        );
        if (existingClaims.Items.Count > 0)
        {
            await _userOperationClaimRepository.DeleteRangeAsync(existingClaims.Items);
        }

        if (claimNames.Count == 0)
        {
            return;
        }

        IPaginate<OperationClaim> operationClaims = await _operationClaimRepository.GetListAsync(
            predicate: claim => claimNames.Contains(claim.Name),
            cancellationToken: cancellationToken
        );

        foreach (OperationClaim operationClaim in operationClaims.Items)
        {
            await _userOperationClaimRepository.AddAsync(
                new UserOperationClaim { UserId = userId, OperationClaimId = operationClaim.Id }
            );
        }
    }
}
