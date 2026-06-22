using NArchitecture.Core.Application.Responses;
using Domain.Enums;

namespace Application.Features.Sites.Commands.Create;

public class CreatedSiteResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public SiteStatus Status { get; set; }
    public string? Description { get; set; }
}