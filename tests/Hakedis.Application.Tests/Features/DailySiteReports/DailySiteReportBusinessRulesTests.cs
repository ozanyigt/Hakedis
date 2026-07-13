using Application.Features.DailySiteReports.Rules;
using Application.Services.CurrentUser;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using Moq;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Hakedis.Application.Tests.Features.DailySiteReports;

public class DailySiteReportBusinessRulesTests
{
    [Fact]
    public async Task Firm_user_cannot_request_another_tenant()
    {
        Guid ownTenantId = Guid.NewGuid();
        Mock<ICurrentUserService> currentUser = new();
        currentUser.SetupGet(x => x.IsGlobalAdmin).Returns(false);
        currentUser
            .Setup(x => x.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { TenantId = ownTenantId });

        DailySiteReportBusinessRules rules = new(
            Mock.Of<IDailySiteReportRepository>(),
            Mock.Of<IProjectRepository>(),
            Mock.Of<ISiteRepository>(),
            currentUser.Object);

        await Assert.ThrowsAsync<BusinessException>(() =>
            rules.ResolveTenantAsync(Guid.NewGuid(), true, CancellationToken.None));
    }

    [Fact]
    public void Draft_report_can_be_submitted()
    {
        DailySiteReport report = new() { Status = DailySiteReportStatus.Draft };

        DailySiteReportBusinessRules.EnsureTransition(report, DailySiteReportStatus.Submitted);
    }

    [Fact]
    public void Submitted_report_can_be_approved_or_rejected()
    {
        DailySiteReport report = new() { Status = DailySiteReportStatus.Submitted };

        DailySiteReportBusinessRules.EnsureTransition(report, DailySiteReportStatus.Approved);
        DailySiteReportBusinessRules.EnsureTransition(report, DailySiteReportStatus.Rejected);
    }

    [Fact]
    public void Invalid_transition_is_rejected()
    {
        DailySiteReport report = new() { Status = DailySiteReportStatus.Draft };

        Assert.Throws<BusinessException>(
            () => DailySiteReportBusinessRules.EnsureTransition(report, DailySiteReportStatus.Approved));
    }

    [Fact]
    public void Approved_report_is_locked()
    {
        DailySiteReport report = new() { Status = DailySiteReportStatus.Approved };

        Assert.Throws<BusinessException>(() => DailySiteReportBusinessRules.EnsureEditable(report));
    }

    [Fact]
    public void Submitted_report_is_locked()
    {
        DailySiteReport report = new() { Status = DailySiteReportStatus.Submitted };

        Assert.Throws<BusinessException>(() => DailySiteReportBusinessRules.EnsureEditable(report));
    }

    [Theory]
    [InlineData(DailySiteReportStatus.Draft)]
    [InlineData(DailySiteReportStatus.Rejected)]
    public void Draft_and_rejected_reports_are_editable(DailySiteReportStatus status)
    {
        DailySiteReport report = new() { Status = status };

        DailySiteReportBusinessRules.EnsureEditable(report);
    }

    [Fact]
    public void Minimum_temperature_cannot_exceed_maximum()
    {
        Assert.Throws<BusinessException>(
            () => DailySiteReportBusinessRules.ValidateTemperatures(25, 10));
    }

    [Fact]
    public void Seventh_photo_is_rejected()
    {
        Assert.Throws<BusinessException>(
            () => DailySiteReportBusinessRules.EnsurePhotoCanBeAdded(6, 1024, "site.jpg", "image/jpeg"));
    }

    [Fact]
    public void Photo_larger_than_8_mb_is_rejected()
    {
        Assert.Throws<BusinessException>(
            () => DailySiteReportBusinessRules.EnsurePhotoCanBeAdded(
                0, 8 * 1024 * 1024 + 1, "site.webp", "image/webp"));
    }
}
