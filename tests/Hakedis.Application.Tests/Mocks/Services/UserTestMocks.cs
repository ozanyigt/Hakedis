using Application.Services.CurrentUser;
using Application.Services.FirmRoles;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace StarterProject.Application.Tests.Mocks.Services;

public static class UserTestMocks
{
    public static ICurrentUserService CreateGlobalAdminCurrentUserService()
    {
        Mock<ICurrentUserService> mock = new();
        mock.Setup(service => service.IsGlobalAdmin).Returns(true);
        mock.Setup(service => service.IsAuthenticated).Returns(true);
        mock.Setup(service => service.HasClaim(It.IsAny<string>())).Returns(true);
        mock.Setup(service => service.RoleClaims).Returns(Array.Empty<string>());
        mock.Setup(service => service.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        return mock.Object;
    }

    public static IFirmRoleAssignmentService CreateFirmRoleAssignmentService()
    {
        Mock<IFirmRoleAssignmentService> mock = new();
        mock.Setup(service =>
                service.AssignAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<FirmRole>(),
                    It.IsAny<FirmRole?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        return mock.Object;
    }
}
