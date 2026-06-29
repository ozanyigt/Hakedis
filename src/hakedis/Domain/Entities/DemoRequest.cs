using Domain.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class DemoRequest : Entity<Guid>
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DemoRequestStatus Status { get; set; } = DemoRequestStatus.New;
}
