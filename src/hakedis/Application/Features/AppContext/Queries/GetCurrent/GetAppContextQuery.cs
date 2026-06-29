using Application.Common.Authorization;
using Application.Services.CurrentUser;
using Application.Services.FirmRoles;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.AppContext.Queries.GetCurrent;

public class GetAppContextQuery : IRequest<GetAppContextResponse>, IAuthenticatedRequest
{
    public Guid? TenantId { get; set; }

    public class GetAppContextQueryHandler : IRequestHandler<GetAppContextQuery, GetAppContextResponse>
    {
        private static readonly string[] AllModules = ["Metraj", "Puantaj", "Hakedis"];

        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public GetAppContextQueryHandler(
            ICurrentUserService currentUserService,
            ITenantRepository tenantRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository
        )
        {
            _currentUserService = currentUserService;
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<GetAppContextResponse> Handle(
            GetAppContextQuery request,
            CancellationToken cancellationToken
        )
        {
            User? user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
            {
                throw new AuthorizationException("You are not authenticated.");
            }

            if (_currentUserService.IsGlobalAdmin)
            {
                GetAppContextResponse adminContext = await BuildAdminContextAsync(request.TenantId, cancellationToken);
                adminContext.IsGlobalAdmin = true;
                return adminContext;
            }

            if (!user.TenantId.HasValue)
            {
                return new GetAppContextResponse
                {
                    IsGlobalAdmin = false,
                    FirmRole = user.FirmRole.HasValue ? (int)user.FirmRole.Value : null,
                    SecondaryFirmRole = user.SecondaryFirmRole.HasValue ? (int)user.SecondaryFirmRole.Value : null,
                };
            }

            Tenant? tenant = await _tenantRepository.GetAsync(
                predicate: item => item.Id == user.TenantId.Value,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (tenant == null)
            {
                return new GetAppContextResponse();
            }

            if (!tenant.IsActive)
            {
                return new GetAppContextResponse
                {
                    IsGlobalAdmin = false,
                    FirmRole = user.FirmRole.HasValue ? (int)user.FirmRole.Value : null,
                    SecondaryFirmRole = user.SecondaryFirmRole.HasValue ? (int)user.SecondaryFirmRole.Value : null,
                    TenantId = tenant.Id,
                    TenantName = tenant.Name,
                    EnabledModules = [],
                    CanSwitchTenant = false,
                    Tenants =
                    [
                        new AppContextTenantItemDto { Id = tenant.Id, Name = tenant.Name },
                    ],
                };
            }

            IReadOnlyList<string> accessibleModules = await ResolveAccessibleModulesAsync(user, cancellationToken);

            return new GetAppContextResponse
            {
                IsGlobalAdmin = false,
                FirmRole = user.FirmRole.HasValue ? (int)user.FirmRole.Value : null,
                SecondaryFirmRole = user.SecondaryFirmRole.HasValue ? (int)user.SecondaryFirmRole.Value : null,
                TenantId = tenant.Id,
                TenantName = tenant.Name,
                EnabledModules = accessibleModules,
                CanSwitchTenant = false,
                Tenants =
                [
                    new AppContextTenantItemDto { Id = tenant.Id, Name = tenant.Name },
                ],
            };
        }

        private async Task<GetAppContextResponse> BuildAdminContextAsync(
            Guid? requestedTenantId,
            CancellationToken cancellationToken
        )
        {
            IPaginate<Tenant> tenantsPage = await _tenantRepository.GetListAsync(
                index: 0,
                size: 200,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            IReadOnlyList<AppContextTenantItemDto> tenants = tenantsPage
                .Items.Select(tenant => new AppContextTenantItemDto { Id = tenant.Id, Name = tenant.Name })
                .ToList();

            Guid? activeTenantId = requestedTenantId;
            if (!activeTenantId.HasValue || tenants.All(tenant => tenant.Id != activeTenantId.Value))
            {
                activeTenantId = tenants.FirstOrDefault()?.Id;
            }

            string? tenantName = tenants.FirstOrDefault(tenant => tenant.Id == activeTenantId)?.Name;

            return new GetAppContextResponse
            {
                IsGlobalAdmin = true,
                TenantId = activeTenantId,
                TenantName = tenantName,
                EnabledModules = AllModules,
                CanSwitchTenant = true,
                Tenants = tenants,
            };
        }

        private async Task<IReadOnlyList<string>> ResolveAccessibleModulesAsync(
            User user,
            CancellationToken cancellationToken
        )
        {
            if (!user.TenantId.HasValue)
            {
                return [];
            }

            IReadOnlyList<string> subscriptionModules = await GetSubscriptionModulesForTenantAsync(
                user.TenantId.Value,
                cancellationToken
            );

            if (!user.FirmRole.HasValue)
            {
                return subscriptionModules;
            }

            IReadOnlyList<string> roleModules = FirmRoleModuleMapper.GetModuleNames(
                user.FirmRole.Value,
                user.SecondaryFirmRole
            );

            return subscriptionModules
                .Where(module => roleModules.Contains(module, StringComparer.Ordinal))
                .ToList();
        }

        private async Task<IReadOnlyList<string>> GetSubscriptionModulesForTenantAsync(
            Guid tenantId,
            CancellationToken cancellationToken
        )
        {
            IPaginate<Subscription> subscriptions = await _subscriptionRepository.GetListAsync(
                predicate: subscription =>
                    subscription.TenantId == tenantId
                    && (
                        subscription.Status == SubscriptionStatus.Active
                        || subscription.Status == SubscriptionStatus.Trial
                    ),
                orderBy: query => query.OrderByDescending(subscription => subscription.StartDate),
                size: 1,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            Subscription? subscription = subscriptions.Items.FirstOrDefault();
            if (subscription == null)
            {
                return [];
            }

            if (subscription.EndDate.HasValue && subscription.EndDate.Value < DateTime.UtcNow)
            {
                await ExpireSubscriptionIfNeededAsync(subscription, cancellationToken);
                return [];
            }

            SubscriptionPlan? plan = await _subscriptionPlanRepository.GetAsync(
                predicate: item => item.Id == subscription.SubscriptionPlanId,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (plan == null || string.IsNullOrWhiteSpace(plan.EnabledModules))
            {
                return [];
            }

            return plan
                .EnabledModules.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(module => FirmRoleModuleMapper.NormalizeModuleName(module))
                .Where(module => module != null)
                .Cast<string>()
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        private async Task ExpireSubscriptionIfNeededAsync(
            Subscription subscription,
            CancellationToken cancellationToken
        )
        {
            if (!subscription.EndDate.HasValue || subscription.EndDate.Value >= DateTime.UtcNow)
            {
                return;
            }

            Subscription? tracked = await _subscriptionRepository.GetAsync(
                predicate: item => item.Id == subscription.Id,
                cancellationToken: cancellationToken
            );

            if (
                tracked != null
                && (tracked.Status == SubscriptionStatus.Active || tracked.Status == SubscriptionStatus.Trial)
            )
            {
                tracked.Status = SubscriptionStatus.Expired;
                await _subscriptionRepository.UpdateAsync(tracked);
            }
        }
    }
}
