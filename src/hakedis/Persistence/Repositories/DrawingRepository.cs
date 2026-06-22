using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class DrawingRepository : EfRepositoryBase<Drawing, Guid, BaseDbContext>, IDrawingRepository
{
    public DrawingRepository(BaseDbContext context) : base(context)
    {
    }
}