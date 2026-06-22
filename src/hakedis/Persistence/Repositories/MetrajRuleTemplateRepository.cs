using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class MetrajRuleTemplateRepository : EfRepositoryBase<MetrajRuleTemplate, Guid, BaseDbContext>, IMetrajRuleTemplateRepository
{
    public MetrajRuleTemplateRepository(BaseDbContext context) : base(context)
    {
    }
}