using Application.Features.Drawings.Commands.Create;
using Application.Features.Drawings.Commands.Delete;
using Application.Features.Drawings.Commands.Update;
using Application.Features.Drawings.Queries.GetById;
using Application.Features.Drawings.Queries.GetList;
using Application.Features.Drawings.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Drawings.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateDrawingCommand, Drawing>();
        CreateMap<Drawing, CreatedDrawingResponse>();

        CreateMap<UpdateDrawingCommand, Drawing>();
        CreateMap<Drawing, UpdatedDrawingResponse>();

        CreateMap<DeleteDrawingCommand, Drawing>();
        CreateMap<Drawing, DeletedDrawingResponse>();

        CreateMap<Drawing, GetByIdDrawingResponse>();

        CreateMap<Drawing, GetListDrawingListItemDto>();
        CreateMap<IPaginate<Drawing>, GetListResponse<GetListDrawingListItemDto>>();

        CreateMap<Drawing, GetListByDynamicDrawingListItemDto>();
        CreateMap<IPaginate<Drawing>, GetListResponse<GetListByDynamicDrawingListItemDto>>();
    }
}