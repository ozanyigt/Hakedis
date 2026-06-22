using Application.Features.Projects.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Pipelines.Authorization;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.Projects.Constants.ProjectsOperationClaims;

namespace Application.Features.Projects.Queries.GetListByDynamic;

public class GetListByDynamicProjectQuery : IRequest<GetListResponse<GetListByDynamicProjectListItemDto>>, ILoggableRequest, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }

    public string[] Roles => [Admin, Read];
        
    

    public class GetListByDynamicProjectQueryHandler : IRequestHandler<GetListByDynamicProjectQuery, GetListResponse<GetListByDynamicProjectListItemDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicProjectQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByDynamicProjectListItemDto>> Handle(GetListByDynamicProjectQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Project> projects = await _projectRepository.GetListByDynamicAsync(
                dynamic: request.Dynamic,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByDynamicProjectListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicProjectListItemDto>>(projects);
            return response;
        }
    }
}
