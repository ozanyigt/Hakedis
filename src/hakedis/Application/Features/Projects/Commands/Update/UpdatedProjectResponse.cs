using NArchitecture.Core.Application.Responses;
using Domain.Enums;

namespace Application.Features.Projects.Commands.Update;

public class UpdatedProjectResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public string? ClientName { get; set; }
    public decimal ContractAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string? Description { get; set; }
}