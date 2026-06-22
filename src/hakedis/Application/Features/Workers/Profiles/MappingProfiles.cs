using Application.Features.Workers.Commands.Create;
using Application.Features.Workers.Commands.Delete;
using Application.Features.Workers.Commands.Update;
using Application.Features.Workers.Queries.GetById;
using Application.Features.Workers.Queries.GetList;
using Application.Features.Workers.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Workers.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateWorkerCommand, Worker>();
        CreateMap<Worker, CreatedWorkerResponse>();

        CreateMap<UpdateWorkerCommand, Worker>();
        CreateMap<Worker, UpdatedWorkerResponse>();

        CreateMap<DeleteWorkerCommand, Worker>();
        CreateMap<Worker, DeletedWorkerResponse>();

        CreateMap<Worker, GetByIdWorkerResponse>();

        CreateMap<Worker, GetListWorkerListItemDto>();
        CreateMap<IPaginate<Worker>, GetListResponse<GetListWorkerListItemDto>>();

        CreateMap<Worker, GetListByDynamicWorkerListItemDto>();
        CreateMap<IPaginate<Worker>, GetListResponse<GetListByDynamicWorkerListItemDto>>();
    }
}