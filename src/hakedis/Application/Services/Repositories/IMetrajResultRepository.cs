using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IMetrajResultRepository : IAsyncRepository<MetrajResult, Guid>, IRepository<MetrajResult, Guid>
{
}