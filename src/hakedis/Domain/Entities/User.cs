namespace Domain.Entities;

public class User : NArchitecture.Core.Security.Entities.User<Guid>
{
    public Guid? TenantId { get; set; }

    public virtual Tenant? Tenant { get; set; }
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;
    public virtual ICollection<PuantajRecord> ApprovedPuantajRecords { get; set; } = new List<PuantajRecord>();
    public virtual ICollection<HakedisPeriod> ApprovedHakedisPeriods { get; set; } = new List<HakedisPeriod>();
}
