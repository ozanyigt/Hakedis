using Application.Features.Projects.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Projects.Rules;

public class ProjectBusinessRules : BaseBusinessRules
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILocalizationService _localizationService;

    public ProjectBusinessRules(IProjectRepository projectRepository, ILocalizationService localizationService)
    {
        _projectRepository = projectRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ProjectsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ProjectShouldExistWhenSelected(Project? project)
    {
        if (project == null)
            await throwBusinessException(ProjectsBusinessMessages.ProjectNotExists);
    }

    public async Task ProjectIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetAsync(
            predicate: p => p.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await ProjectShouldExistWhenSelected(project);
    }
}