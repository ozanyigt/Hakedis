using Application.Features.Tenants.Commands.Create;
using Application.Features.Tenants.Commands.Delete;
using Application.Features.Tenants.Commands.Update;
using Application.Features.Tenants.Queries.GetById;
using Application.Features.Tenants.Queries.GetList;
using Application.Features.Tenants.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Tenants.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateTenantCommand, Tenant>();
        CreateMap<Tenant, CreatedTenantResponse>();

        CreateMap<UpdateTenantCommand, Tenant>();
        CreateMap<Tenant, UpdatedTenantResponse>();

        CreateMap<DeleteTenantCommand, Tenant>();
        CreateMap<Tenant, DeletedTenantResponse>();

        CreateMap<Tenant, GetByIdTenantResponse>();

        CreateMap<Tenant, GetListTenantListItemDto>();
        CreateMap<IPaginate<Tenant>, GetListResponse<GetListTenantListItemDto>>();

        CreateMap<Tenant, GetListByDynamicTenantListItemDto>();
        CreateMap<IPaginate<Tenant>, GetListResponse<GetListByDynamicTenantListItemDto>>();
    }
}