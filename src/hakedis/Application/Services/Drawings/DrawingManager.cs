using Application.Features.Drawings.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Drawings;

public class DrawingManager : IDrawingService
{
    private readonly IDrawingRepository _drawingRepository;
    private readonly DrawingBusinessRules _drawingBusinessRules;

    public DrawingManager(IDrawingRepository drawingRepository, DrawingBusinessRules drawingBusinessRules)
    {
        _drawingRepository = drawingRepository;
        _drawingBusinessRules = drawingBusinessRules;
    }

    public async Task<Drawing?> GetAsync(
        Expression<Func<Drawing, bool>> predicate,
        Func<IQueryable<Drawing>, IIncludableQueryable<Drawing, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Drawing? drawing = await _drawingRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return drawing;
    }

    public async Task<IPaginate<Drawing>?> GetListAsync(
        Expression<Func<Drawing, bool>>? predicate = null,
        Func<IQueryable<Drawing>, IOrderedQueryable<Drawing>>? orderBy = null,
        Func<IQueryable<Drawing>, IIncludableQueryable<Drawing, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Drawing> drawingList = await _drawingRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return drawingList;
    }

    public async Task<Drawing> AddAsync(Drawing drawing)
    {
        Drawing addedDrawing = await _drawingRepository.AddAsync(drawing);

        return addedDrawing;
    }

    public async Task<Drawing> UpdateAsync(Drawing drawing)
    {
        Drawing updatedDrawing = await _drawingRepository.UpdateAsync(drawing);

        return updatedDrawing;
    }

    public async Task<Drawing> DeleteAsync(Drawing drawing, bool permanent = false)
    {
        Drawing deletedDrawing = await _drawingRepository.DeleteAsync(drawing);

        return deletedDrawing;
    }
}
