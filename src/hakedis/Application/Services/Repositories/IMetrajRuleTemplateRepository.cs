using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IMetrajRuleTemplateRepository : IAsyncRepository<MetrajRuleTemplate, Guid>, IRepository<MetrajRuleTemplate, Guid>
{
}