using Domain.Enums;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.UpdateFirmRole;

public class UpdatedUserFirmRoleResponse : IResponse
{
    public Guid Id { get; set; }
    public FirmRole FirmRole { get; set; }
    public FirmRole? SecondaryFirmRole { get; set; }
}
