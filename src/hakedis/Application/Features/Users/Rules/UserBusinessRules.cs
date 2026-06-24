using Application.Features.Users.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using NArchitecture.Core.Security.Hashing;
using Application.Services.CurrentUser;

namespace Application.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;
    private readonly ILocalizationService _localizationService;
    private readonly ICurrentUserService _currentUserService;

    public UserBusinessRules(
        IUserRepository userRepository,
        ILocalizationService localizationService,
        ICurrentUserService currentUserService
    )
    {
        _userRepository = userRepository;
        _localizationService = localizationService;
        _currentUserService = currentUserService;
    }

    public UserBusinessRules(IUserRepository userRepository, ILocalizationService localizationService)
        : this(userRepository, localizationService, new FallbackCurrentUserService()) { }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UsersMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null)
            await throwBusinessException(UsersMessages.UserDontExists);
    }

    public async Task UserIdShouldBeExistsWhenSelected(Guid id)
    {
        bool doesExist = await _userRepository.AnyAsync(predicate: u => u.Id == id);
        if (doesExist)
            await throwBusinessException(UsersMessages.UserDontExists);
    }

    public async Task UserPasswordShouldBeMatched(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            await throwBusinessException(UsersMessages.PasswordDontMatch);
    }

    public async Task UserEmailShouldNotExistsWhenInsert(string email)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Email == email);
        if (doesExists)
            await throwBusinessException(UsersMessages.UserMailAlreadyExists);
    }

    public async Task UserEmailShouldNotExistsWhenUpdate(Guid id, string email)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Id != id && u.Email == email);
        if (doesExists)
            await throwBusinessException(UsersMessages.UserMailAlreadyExists);
    }

    public async Task TenantAndFirmRoleShouldBeValidForCreate(Guid? tenantId, FirmRole? firmRole)
    {
        if (_currentUserService.IsGlobalAdmin)
        {
            if (!tenantId.HasValue)
                await throwBusinessException(UsersMessages.TenantRequired);
            if (!firmRole.HasValue)
                await throwBusinessException(UsersMessages.FirmRoleRequired);
            return;
        }

        User? caller = await _currentUserService.GetCurrentUserAsync();
        if (caller?.TenantId == null)
            await throwBusinessException(UsersMessages.UnauthorizedTenantAccess);

        if (!firmRole.HasValue)
            await throwBusinessException(UsersMessages.FirmRoleRequired);

        if (!_currentUserService.HasClaim(UsersOperationClaims.Create))
            await throwBusinessException(UsersMessages.UnauthorizedTenantAccess);
    }

    public async Task<Guid> ResolveTenantIdForCreateAsync(Guid? requestedTenantId)
    {
        if (_currentUserService.IsGlobalAdmin)
        {
            if (!requestedTenantId.HasValue)
                await throwBusinessException(UsersMessages.TenantRequired);
            return requestedTenantId.Value;
        }

        User? caller = await _currentUserService.GetCurrentUserAsync();
        if (caller?.TenantId == null)
            await throwBusinessException(UsersMessages.UnauthorizedTenantAccess);

        return caller.TenantId.Value;
    }

    public async Task UserShouldBeManageableByCaller(User user)
    {
        if (_currentUserService.IsGlobalAdmin)
        {
            return;
        }

        if (user.TenantId == null)
            await throwBusinessException(UsersMessages.CannotManageGlobalAdmin);

        User? caller = await _currentUserService.GetCurrentUserAsync();
        if (caller?.TenantId == null || user.TenantId != caller.TenantId)
            await throwBusinessException(UsersMessages.UnauthorizedTenantAccess);

        if (!_currentUserService.HasClaim(UsersOperationClaims.Read))
            await throwBusinessException(UsersMessages.UnauthorizedTenantAccess);
    }
}
