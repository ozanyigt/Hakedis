using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class HakedisDeductionLineRepository
    : EfRepositoryBase<HakedisDeductionLine, Guid, BaseDbContext>,
        IHakedisDeductionLineRepository
{
    public HakedisDeductionLineRepository(BaseDbContext context)
        : base(context) { }
}
