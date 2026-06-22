using Application.Features.Projects.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Projects;

public class ProjectManager : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ProjectBusinessRules _projectBusinessRules;

    public ProjectManager(IProjectRepository projectRepository, ProjectBusinessRules projectBusinessRules)
    {
        _projectRepository = projectRepository;
        _projectBusinessRules = projectBusinessRules;
    }

    public async Task<Project?> GetAsync(
        Expression<Func<Project, bool>> predicate,
        Func<IQueryable<Project>, IIncludableQueryable<Project, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Project? project = await _projectRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return project;
    }

    public async Task<IPaginate<Project>?> GetListAsync(
        Expression<Func<Project, bool>>? predicate = null,
        Func<IQueryable<Project>, IOrderedQueryable<Project>>? orderBy = null,
        Func<IQueryable<Project>, IIncludableQueryable<Project, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Project> projectList = await _projectRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return projectList;
    }

    public async Task<Project> AddAsync(Project project)
    {
        Project addedProject = await _projectRepository.AddAsync(project);

        return addedProject;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        Project updatedProject = await _projectRepository.UpdateAsync(project);

        return updatedProject;
    }

    public async Task<Project> DeleteAsync(Project project, bool permanent = false)
    {
        Project deletedProject = await _projectRepository.DeleteAsync(project);

        return deletedProject;
    }
}
