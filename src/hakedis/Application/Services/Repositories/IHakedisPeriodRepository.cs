using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IHakedisPeriodRepository : IAsyncRepository<HakedisPeriod, Guid>, IRepository<HakedisPeriod, Guid>
{
}