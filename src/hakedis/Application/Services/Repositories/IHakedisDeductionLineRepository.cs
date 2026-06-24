using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IHakedisDeductionLineRepository
    : IAsyncRepository<HakedisDeductionLine, Guid>,
        IRepository<HakedisDeductionLine, Guid> { }
