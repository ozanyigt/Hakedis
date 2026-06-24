using Application.Features.HakedisDeductionLines.Commands.Create;
using Application.Features.HakedisDeductionLines.Commands.Delete;
using Application.Features.HakedisDeductionLines.Commands.Update;
using Application.Features.HakedisDeductionLines.Queries.GetById;
using Application.Features.HakedisDeductionLines.Queries.GetListByDynamic;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.HakedisDeductionLines.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateHakedisDeductionLineCommand, HakedisDeductionLine>();
        CreateMap<HakedisDeductionLine, CreatedHakedisDeductionLineResponse>();

        CreateMap<UpdateHakedisDeductionLineCommand, HakedisDeductionLine>();
        CreateMap<HakedisDeductionLine, UpdatedHakedisDeductionLineResponse>();

        CreateMap<HakedisDeductionLine, DeletedHakedisDeductionLineResponse>();
        CreateMap<HakedisDeductionLine, GetByIdHakedisDeductionLineResponse>();

        CreateMap<HakedisDeductionLine, GetListByDynamicHakedisDeductionLineListItemDto>();
        CreateMap<IPaginate<HakedisDeductionLine>, GetListResponse<GetListByDynamicHakedisDeductionLineListItemDto>>();
    }
}
