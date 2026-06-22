using NArchitecture.Core.Application.Responses;

namespace Application.Features.MetrajRuleTemplates.Commands.Delete;

public class DeletedMetrajRuleTemplateResponse : IResponse
{
    public Guid Id { get; set; }
}