using Application.Features.MetrajResults.Commands.Create;
using Application.Features.MetrajResults.Commands.Delete;
using Application.Features.MetrajResults.Commands.Update;
using Application.Features.MetrajResults.Queries.GetById;
using Application.Features.MetrajResults.Queries.GetList;
using Application.Features.MetrajResults.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.MetrajResults.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateMetrajResultCommand, MetrajResult>();
        CreateMap<MetrajResult, CreatedMetrajResultResponse>();

        CreateMap<UpdateMetrajResultCommand, MetrajResult>();
        CreateMap<MetrajResult, UpdatedMetrajResultResponse>();

        CreateMap<DeleteMetrajResultCommand, MetrajResult>();
        CreateMap<MetrajResult, DeletedMetrajResultResponse>();

        CreateMap<MetrajResult, GetByIdMetrajResultResponse>();

        CreateMap<MetrajResult, GetListMetrajResultListItemDto>();
        CreateMap<IPaginate<MetrajResult>, GetListResponse<GetListMetrajResultListItemDto>>();

        CreateMap<MetrajResult, GetListByDynamicMetrajResultListItemDto>();
        CreateMap<IPaginate<MetrajResult>, GetListResponse<GetListByDynamicMetrajResultListItemDto>>();
    }
}