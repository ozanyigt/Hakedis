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

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommand : IRequest<CreatedProjectResponse>, ISecuredRequest, ILoggableRequest, ITransactionalRequest
{
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

    public string[] Roles => [Admin, Write, ProjectsOperationClaims.Create];

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreatedProjectResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectBusinessRules _projectBusinessRules;

        public CreateProjectCommandHandler(IMapper mapper, IProjectRepository projectRepository,
                                         ProjectBusinessRules projectBusinessRules)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _projectBusinessRules = projectBusinessRules;
        }

        public async Task<CreatedProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            Project project = _mapper.Map<Project>(request);

            await _projectRepository.AddAsync(project);

            CreatedProjectResponse response = _mapper.Map<CreatedProjectResponse>(project);
            return response;
        }
    }
}