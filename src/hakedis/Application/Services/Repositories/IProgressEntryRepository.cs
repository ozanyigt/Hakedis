using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IProgressEntryRepository : IAsyncRepository<ProgressEntry, Guid>, IRepository<ProgressEntry, Guid>
{
}