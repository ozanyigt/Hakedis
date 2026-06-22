using Application.Features.Projects.Constants;
using Application.Features.Projects.Constants;
using Application.Features.Projects.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Projects.Constants.ProjectsOperationClaims;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommand : IRequest<DeletedProjectResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, ProjectsOperationClaims.Delete];

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, DeletedProjectResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectBusinessRules _projectBusinessRules;

        public DeleteProjectCommandHandler(IMapper mapper, IProjectRepository projectRepository,
                                         ProjectBusinessRules projectBusinessRules)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _projectBusinessRules = projectBusinessRules;
        }

        public async Task<DeletedProjectResponse> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetAsync(predicate: p => p.Id == request.Id, cancellationToken: cancellationToken);
            await _projectBusinessRules.ProjectShouldExistWhenSelected(project);

            await _projectRepository.DeleteAsync(project!);

            DeletedProjectResponse response = _mapper.Map<DeletedProjectResponse>(project);
            return response;
        }
    }
}