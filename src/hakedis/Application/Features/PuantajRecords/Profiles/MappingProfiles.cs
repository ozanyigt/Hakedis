using Application.Features.PuantajRecords.Commands.Create;
using Application.Features.PuantajRecords.Commands.Delete;
using Application.Features.PuantajRecords.Commands.Update;
using Application.Features.PuantajRecords.Queries.GetById;
using Application.Features.PuantajRecords.Queries.GetList;
using Application.Features.PuantajRecords.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.PuantajRecords.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreatePuantajRecordCommand, PuantajRecord>();
        CreateMap<PuantajRecord, CreatedPuantajRecordResponse>();

        CreateMap<UpdatePuantajRecordCommand, PuantajRecord>();
        CreateMap<PuantajRecord, UpdatedPuantajRecordResponse>();

        CreateMap<DeletePuantajRecordCommand, PuantajRecord>();
        CreateMap<PuantajRecord, DeletedPuantajRecordResponse>();

        CreateMap<PuantajRecord, GetByIdPuantajRecordResponse>();

        CreateMap<PuantajRecord, GetListPuantajRecordListItemDto>();
        CreateMap<IPaginate<PuantajRecord>, GetListResponse<GetListPuantajRecordListItemDto>>();

        CreateMap<PuantajRecord, GetListByDynamicPuantajRecordListItemDto>();
        CreateMap<IPaginate<PuantajRecord>, GetListResponse<GetListByDynamicPuantajRecordListItemDto>>();
    }
}