using Domain.Enums;

namespace Application.Services.FirmRoles;

public interface IFirmRoleAssignmentService
{
    Task AssignAsync(
        Guid userId,
        FirmRole primaryRole,
        FirmRole? secondaryRole,
        CancellationToken cancellationToken = default
    );
}
