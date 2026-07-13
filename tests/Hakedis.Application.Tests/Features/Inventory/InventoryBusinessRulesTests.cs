using Application.Features.Inventory;
using Application.Services.CurrentUser;
using Application.Services.Inventory;
using Application.Services.Repositories;
using Domain.Entities;
using Moq;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Hakedis.Application.Tests.Features.Inventory;

public class InventoryBusinessRulesTests
{
    [Fact]
    public async Task Tenant_user_cannot_access_another_tenants_inventory()
    {
        Guid tenantId = Guid.NewGuid();
        Mock<ICurrentUserService> currentUser = new();
        currentUser.SetupGet(x => x.IsGlobalAdmin).Returns(false);
        currentUser.Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { TenantId = tenantId });
        InventoryBusinessRules rules = CreateRules(currentUser.Object);

        await Assert.ThrowsAsync<BusinessException>(() =>
            rules.ResolveTenantAsync(Guid.NewGuid(), true, CancellationToken.None));
    }

    [Fact]
    public async Task Global_admin_must_specify_tenant_for_writes()
    {
        Mock<ICurrentUserService> currentUser = new();
        currentUser.SetupGet(x => x.IsGlobalAdmin).Returns(true);
        InventoryBusinessRules rules = CreateRules(currentUser.Object);

        await Assert.ThrowsAsync<BusinessException>(() =>
            rules.ResolveTenantAsync(null, true, CancellationToken.None));
    }

    [Fact]
    public void Receipt_recalculates_moving_weighted_average()
    {
        (decimal quantity, decimal average) =
            StockValuationCalculator.Receive(10m, 100m, 5m, 160m);

        Assert.Equal(15m, quantity);
        Assert.Equal(120m, average);
    }

    [Fact]
    public void Consumption_keeps_average_cost_and_returns_issue_cost()
    {
        (decimal quantity, decimal average, decimal issueCost) =
            StockValuationCalculator.Issue(15m, 120m, 4m);

        Assert.Equal(11m, quantity);
        Assert.Equal(120m, average);
        Assert.Equal(120m, issueCost);
    }

    [Fact]
    public void Consumption_cannot_make_stock_negative()
    {
        Assert.Throws<BusinessException>(() =>
            StockValuationCalculator.Issue(2m, 120m, 3m));
    }

    private static InventoryBusinessRules CreateRules(ICurrentUserService currentUser) =>
        new(Mock.Of<IMaterialRepository>(), Mock.Of<IProjectRepository>(),
            Mock.Of<ISiteRepository>(), currentUser);
}
