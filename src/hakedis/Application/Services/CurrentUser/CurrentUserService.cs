using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using NArchitecture.Core.Security.Constants;
using NArchitecture.Core.Security.Extensions;

namespace Application.Services.CurrentUser;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public Guid? UserId
    {
        get
        {
            string? id = _httpContextAccessor.HttpContext?.User.GetIdClaim();
            return Guid.TryParse(id, out Guid userId) ? userId : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public bool IsGlobalAdmin => RoleClaims.Contains(GeneralOperationClaims.Admin);

    public IReadOnlyCollection<string> RoleClaims =>
        _httpContextAccessor.HttpContext?.User.GetRoleClaims()?.ToArray() ?? [];

    public bool HasClaim(string claimName)
    {
        if (IsGlobalAdmin)
        {
            return true;
        }

        if (RoleClaims.Contains(claimName))
        {
            return true;
        }

        string section = claimName.Split('.')[0];
        return RoleClaims.Contains($"{section}.Admin");
    }

    public async Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (!UserId.HasValue)
        {
            return null;
        }

        return await _userRepository.GetAsync(
            predicate: user => user.Id == UserId.Value,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
    }
}
