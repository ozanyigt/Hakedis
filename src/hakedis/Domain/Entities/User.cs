using Domain.Enums;

namespace Domain.Entities;

public class User : NArchitecture.Core.Security.Entities.User<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public FirmRole? FirmRole { get; set; }
    public FirmRole? SecondaryFirmRole { get; set; }

    public virtual Tenant? Tenant { get; set; }
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;
    public virtual ICollection<PuantajRecord> ApprovedPuantajRecords { get; set; } = new List<PuantajRecord>();
    public virtual ICollection<DailySiteReport> CreatedDailySiteReports { get; set; } = new List<DailySiteReport>();
    public virtual ICollection<DailySiteReport> ApprovedDailySiteReports { get; set; } = new List<DailySiteReport>();
    public virtual ICollection<HakedisPeriod> ApprovedHakedisPeriods { get; set; } = new List<HakedisPeriod>();
}
