using Domain.Entities;

namespace Application.Services.CurrentUser;

/// <summary>
/// Used when UserBusinessRules is constructed without HTTP context (e.g. legacy tests).
/// </summary>
internal sealed class FallbackCurrentUserService : ICurrentUserService
{
    public Guid? UserId => null;

    public bool IsAuthenticated => false;

    public bool IsGlobalAdmin => true;

    public IReadOnlyCollection<string> RoleClaims => [];

    public bool HasClaim(string claimName) => true;

    public Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<User?>(null);
}
