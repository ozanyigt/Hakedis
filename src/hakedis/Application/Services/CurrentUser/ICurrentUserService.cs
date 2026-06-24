using Domain.Entities;

namespace Application.Services.CurrentUser;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
    bool IsGlobalAdmin { get; }
    IReadOnlyCollection<string> RoleClaims { get; }
    bool HasClaim(string claimName);
    Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
