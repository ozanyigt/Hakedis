using NArchitecture.Core.Application.Responses;

namespace Application.Features.HakedisPeriods.Commands.Delete;

public class DeletedHakedisPeriodResponse : IResponse
{
    public Guid Id { get; set; }
}