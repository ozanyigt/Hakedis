using Application.Features.Sites.Commands.Create;
using Application.Features.Sites.Commands.Delete;
using Application.Features.Sites.Commands.Update;
using Application.Features.Sites.Queries.GetById;
using Application.Features.Sites.Queries.GetList;
using Application.Features.Sites.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Sites.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateSiteCommand, Site>();
        CreateMap<Site, CreatedSiteResponse>();

        CreateMap<UpdateSiteCommand, Site>();
        CreateMap<Site, UpdatedSiteResponse>();

        CreateMap<DeleteSiteCommand, Site>();
        CreateMap<Site, DeletedSiteResponse>();

        CreateMap<Site, GetByIdSiteResponse>();

        CreateMap<Site, GetListSiteListItemDto>();
        CreateMap<IPaginate<Site>, GetListResponse<GetListSiteListItemDto>>();

        CreateMap<Site, GetListByDynamicSiteListItemDto>();
        CreateMap<IPaginate<Site>, GetListResponse<GetListByDynamicSiteListItemDto>>();
    }
}