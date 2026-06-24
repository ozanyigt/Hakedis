using Application.Features.MetrajResults.Constants;
using Application.Features.Projects.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.MetrajResults.Constants.MetrajResultsOperationClaims;

namespace Application.Features.ProjectMetrajLayerMappings.Queries.GetByProject;

public class GetProjectMetrajLayerMappingsQuery : IRequest<IReadOnlyList<ProjectMetrajLayerMappingDto>>, ISecuredRequest
{
    public Guid ProjectId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetProjectMetrajLayerMappingsQueryHandler
        : IRequestHandler<GetProjectMetrajLayerMappingsQuery, IReadOnlyList<ProjectMetrajLayerMappingDto>>
    {
        private readonly IProjectMetrajLayerMappingRepository _mappingRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectBusinessRules _projectBusinessRules;

        public GetProjectMetrajLayerMappingsQueryHandler(
            IProjectMetrajLayerMappingRepository mappingRepository,
            IProjectRepository projectRepository,
            ProjectBusinessRules projectBusinessRules
        )
        {
            _mappingRepository = mappingRepository;
            _projectRepository = projectRepository;
            _projectBusinessRules = projectBusinessRules;
        }

        public async Task<IReadOnlyList<ProjectMetrajLayerMappingDto>> Handle(
            GetProjectMetrajLayerMappingsQuery request,
            CancellationToken cancellationToken
        )
        {
            Project? project = await _projectRepository.GetAsync(
                predicate: item => item.Id == request.ProjectId,
                cancellationToken: cancellationToken
            );
            await _projectBusinessRules.ProjectShouldExistWhenSelected(project);

            IPaginate<ProjectMetrajLayerMapping> mappings = await _mappingRepository.GetListAsync(
                predicate: item => item.ProjectId == request.ProjectId,
                index: 0,
                size: 20,
                cancellationToken: cancellationToken
            );

            Dictionary<MetrajKalemType, ProjectMetrajLayerMapping> mappingByType = mappings
                .Items.ToDictionary(item => item.KalemType);

            return Enum.GetValues<MetrajKalemType>()
                .Select(kalemType =>
                {
                    mappingByType.TryGetValue(kalemType, out ProjectMetrajLayerMapping? mapping);
                    return new ProjectMetrajLayerMappingDto
                    {
                        KalemType = kalemType,
                        LayerNames = SplitLayerNames(mapping?.LayerNames),
                    };
                })
                .ToList();
        }

        private static IReadOnlyList<string> SplitLayerNames(string? layerNames) =>
            string.IsNullOrWhiteSpace(layerNames)
                ? []
                : layerNames
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
    }
}

public class ProjectMetrajLayerMappingDto
{
    public MetrajKalemType KalemType { get; set; }
    public IReadOnlyList<string> LayerNames { get; set; } = [];
}
