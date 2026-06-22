using Application.Features.Projects.Commands.Create;
using Application.Features.Projects.Commands.Delete;
using Application.Features.Projects.Commands.Update;
using Application.Features.Projects.Queries.GetById;
using Application.Features.Projects.Queries.GetList;
using Application.Features.Projects.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Projects.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateProjectCommand, Project>();
        CreateMap<Project, CreatedProjectResponse>();

        CreateMap<UpdateProjectCommand, Project>();
        CreateMap<Project, UpdatedProjectResponse>();

        CreateMap<DeleteProjectCommand, Project>();
        CreateMap<Project, DeletedProjectResponse>();

        CreateMap<Project, GetByIdProjectResponse>();

        CreateMap<Project, GetListProjectListItemDto>();
        CreateMap<IPaginate<Project>, GetListResponse<GetListProjectListItemDto>>();

        CreateMap<Project, GetListByDynamicProjectListItemDto>();
        CreateMap<IPaginate<Project>, GetListResponse<GetListByDynamicProjectListItemDto>>();
    }
}