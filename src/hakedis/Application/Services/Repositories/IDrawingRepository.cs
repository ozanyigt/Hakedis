using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IDrawingRepository : IAsyncRepository<Drawing, Guid>, IRepository<Drawing, Guid>
{
}