using NArchitecture.Core.Application.Responses;

namespace Application.Features.ContractItems.Commands.Delete;

public class DeletedContractItemResponse : IResponse
{
    public Guid Id { get; set; }
}