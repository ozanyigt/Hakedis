using Application.Features.Projects.Constants;
using Application.Features.Projects.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using Domain.Enums;
using static Application.Features.Projects.Constants.ProjectsOperationClaims;

namespace Application.Features.Projects.Commands.Update;

public class UpdateProjectCommand : IRequest<UpdatedProjectResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public string? ClientName { get; set; }
    public required decimal ContractAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required ProjectStatus Status { get; set; }
    public string? Description { get; set; }

    public string[] Roles => [Admin, Write, ProjectsOperationClaims.Update];

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, UpdatedProjectResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectBusinessRules _projectBusinessRules;

        public UpdateProjectCommandHandler(IMapper mapper, IProjectRepository projectRepository,
                                         ProjectBusinessRules projectBusinessRules)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _projectBusinessRules = projectBusinessRules;
        }

        public async Task<UpdatedProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetAsync(predicate: p => p.Id == request.Id, cancellationToken: cancellationToken);
            await _projectBusinessRules.ProjectShouldExistWhenSelected(project);
            project = _mapper.Map(request, project);

            await _projectRepository.UpdateAsync(project!);

            UpdatedProjectResponse response = _mapper.Map<UpdatedProjectResponse>(project);
            return response;
        }
    }
}