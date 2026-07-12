using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.FirmRoles;

public class FirmRoleAssignmentService : IFirmRoleAssignmentService
{
    private const int ClaimPageSize = 500;

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

        await ReplaceUserClaimsAsync(userId, claimNames, cancellationToken);
    }

    private async Task ReplaceUserClaimsAsync(
        Guid userId,
        IReadOnlyList<string> claimNames,
        CancellationToken cancellationToken
    )
    {
        // Eski claim'leri tamamen sil (varsayılan sayfa boyutu yüzünden yarım kalmasın)
        while (true)
        {
            IPaginate<UserOperationClaim> existingClaims = await _userOperationClaimRepository.GetListAsync(
                predicate: claim => claim.UserId == userId,
                size: ClaimPageSize,
                cancellationToken: cancellationToken
            );

            if (existingClaims.Items.Count == 0)
            {
                break;
            }

            await _userOperationClaimRepository.DeleteRangeAsync(existingClaims.Items);

            if (existingClaims.Items.Count < ClaimPageSize)
            {
                break;
            }
        }

        if (claimNames.Count == 0)
        {
            return;
        }

        // Rolün gerektirdiği TÜM operation claim'leri getir
        IPaginate<OperationClaim> operationClaims = await _operationClaimRepository.GetListAsync(
            predicate: claim => claimNames.Contains(claim.Name),
            size: Math.Max(claimNames.Count, ClaimPageSize),
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
