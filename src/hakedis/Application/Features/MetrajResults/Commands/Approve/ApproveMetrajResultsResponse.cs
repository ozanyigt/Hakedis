using Domain.Enums;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.MetrajResults.Commands.Approve;

public class ApproveMetrajResultsResponse : IResponse
{
    public Guid DrawingId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DrawingStatus Status { get; set; }
    public int ApprovedCount { get; set; }
}
