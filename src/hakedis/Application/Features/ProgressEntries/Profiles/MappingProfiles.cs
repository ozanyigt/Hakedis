using Application.Features.ProgressEntries.Commands.Create;
using Application.Features.ProgressEntries.Commands.Delete;
using Application.Features.ProgressEntries.Commands.Update;
using Application.Features.ProgressEntries.Queries.GetById;
using Application.Features.ProgressEntries.Queries.GetList;
using Application.Features.ProgressEntries.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.ProgressEntries.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateProgressEntryCommand, ProgressEntry>();
        CreateMap<ProgressEntry, CreatedProgressEntryResponse>();

        CreateMap<UpdateProgressEntryCommand, ProgressEntry>();
        CreateMap<ProgressEntry, UpdatedProgressEntryResponse>();

        CreateMap<DeleteProgressEntryCommand, ProgressEntry>();
        CreateMap<ProgressEntry, DeletedProgressEntryResponse>();

        CreateMap<ProgressEntry, GetByIdProgressEntryResponse>();

        CreateMap<ProgressEntry, GetListProgressEntryListItemDto>();
        CreateMap<IPaginate<ProgressEntry>, GetListResponse<GetListProgressEntryListItemDto>>();

        CreateMap<ProgressEntry, GetListByDynamicProgressEntryListItemDto>();
        CreateMap<IPaginate<ProgressEntry>, GetListResponse<GetListByDynamicProgressEntryListItemDto>>();
    }
}