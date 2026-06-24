using Application.Features.MetrajResults.Constants;
using Application.Features.Projects.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.ProjectMetrajLayerMappings.Commands.Save;

public class SaveProjectMetrajLayerMappingsCommand : IRequest<IReadOnlyList<ProjectMetrajLayerMappingItemDto>>, ISecuredRequest
{
    public Guid ProjectId { get; set; }
    public IList<ProjectMetrajLayerMappingItemDto> Mappings { get; set; } = [];

    public string[] Roles => [Admin, Write, Update];

    public class SaveProjectMetrajLayerMappingsCommandHandler
        : IRequestHandler<SaveProjectMetrajLayerMappingsCommand, IReadOnlyList<ProjectMetrajLayerMappingItemDto>>
    {
        private readonly IProjectMetrajLayerMappingRepository _mappingRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectBusinessRules _projectBusinessRules;

        public SaveProjectMetrajLayerMappingsCommandHandler(
            IProjectMetrajLayerMappingRepository mappingRepository,
            IProjectRepository projectRepository,
            ProjectBusinessRules projectBusinessRules
        )
        {
            _mappingRepository = mappingRepository;
            _projectRepository = projectRepository;
            _projectBusinessRules = projectBusinessRules;
        }

        public async Task<IReadOnlyList<ProjectMetrajLayerMappingItemDto>> Handle(
            SaveProjectMetrajLayerMappingsCommand request,
            CancellationToken cancellationToken
        )
        {
            Project? project = await _projectRepository.GetAsync(
                predicate: item => item.Id == request.ProjectId,
                cancellationToken: cancellationToken
            );
            await _projectBusinessRules.ProjectShouldExistWhenSelected(project);

            IPaginate<ProjectMetrajLayerMapping> existingMappings = await _mappingRepository.GetListAsync(
                predicate: item => item.ProjectId == request.ProjectId,
                index: 0,
                size: 20,
                cancellationToken: cancellationToken
            );

            Dictionary<MetrajKalemType, ProjectMetrajLayerMapping> existingByType = existingMappings
                .Items.ToDictionary(item => item.KalemType);

            foreach (MetrajKalemType kalemType in Enum.GetValues<MetrajKalemType>())
            {
                ProjectMetrajLayerMappingItemDto? incoming = request.Mappings.FirstOrDefault(
                    item => item.KalemType == kalemType
                );
                string layerNames = JoinLayerNames(incoming?.LayerNames ?? []);

                if (existingByType.TryGetValue(kalemType, out ProjectMetrajLayerMapping? existing))
                {
                    existing.LayerNames = layerNames;
                    await _mappingRepository.UpdateAsync(existing);
                    continue;
                }

                if (layerNames.Length == 0)
                    continue;

                await _mappingRepository.AddAsync(
                    new ProjectMetrajLayerMapping
                    {
                        Id = Guid.NewGuid(),
                        TenantId = project!.TenantId,
                        ProjectId = project.Id,
                        KalemType = kalemType,
                        LayerNames = layerNames,
                    }
                );
            }

            return request.Mappings.ToList();
        }

        private static string JoinLayerNames(IEnumerable<string> layerNames) =>
            string.Join(
                ",",
                layerNames
                    .Select(name => name.Trim())
                    .Where(name => name.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
            );
    }
}

public class ProjectMetrajLayerMappingItemDto
{
    public MetrajKalemType KalemType { get; set; }
    public IList<string> LayerNames { get; set; } = [];
}
