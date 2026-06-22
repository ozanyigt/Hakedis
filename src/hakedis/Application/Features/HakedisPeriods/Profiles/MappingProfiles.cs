using Application.Features.HakedisPeriods.Commands.Create;
using Application.Features.HakedisPeriods.Commands.Delete;
using Application.Features.HakedisPeriods.Commands.Update;
using Application.Features.HakedisPeriods.Queries.GetById;
using Application.Features.HakedisPeriods.Queries.GetList;
using Application.Features.HakedisPeriods.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.HakedisPeriods.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateHakedisPeriodCommand, HakedisPeriod>();
        CreateMap<HakedisPeriod, CreatedHakedisPeriodResponse>();

        CreateMap<UpdateHakedisPeriodCommand, HakedisPeriod>();
        CreateMap<HakedisPeriod, UpdatedHakedisPeriodResponse>();

        CreateMap<DeleteHakedisPeriodCommand, HakedisPeriod>();
        CreateMap<HakedisPeriod, DeletedHakedisPeriodResponse>();

        CreateMap<HakedisPeriod, GetByIdHakedisPeriodResponse>();

        CreateMap<HakedisPeriod, GetListHakedisPeriodListItemDto>();
        CreateMap<IPaginate<HakedisPeriod>, GetListResponse<GetListHakedisPeriodListItemDto>>();

        CreateMap<HakedisPeriod, GetListByDynamicHakedisPeriodListItemDto>();
        CreateMap<IPaginate<HakedisPeriod>, GetListResponse<GetListByDynamicHakedisPeriodListItemDto>>();
    }
}