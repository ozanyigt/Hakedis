using Application.Features.MetrajRuleTemplates.Commands.Create;
using Application.Features.MetrajRuleTemplates.Commands.Delete;
using Application.Features.MetrajRuleTemplates.Commands.Update;
using Application.Features.MetrajRuleTemplates.Queries.GetById;
using Application.Features.MetrajRuleTemplates.Queries.GetList;
using Application.Features.MetrajRuleTemplates.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.MetrajRuleTemplates.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateMetrajRuleTemplateCommand, MetrajRuleTemplate>();
        CreateMap<MetrajRuleTemplate, CreatedMetrajRuleTemplateResponse>();

        CreateMap<UpdateMetrajRuleTemplateCommand, MetrajRuleTemplate>();
        CreateMap<MetrajRuleTemplate, UpdatedMetrajRuleTemplateResponse>();

        CreateMap<DeleteMetrajRuleTemplateCommand, MetrajRuleTemplate>();
        CreateMap<MetrajRuleTemplate, DeletedMetrajRuleTemplateResponse>();

        CreateMap<MetrajRuleTemplate, GetByIdMetrajRuleTemplateResponse>();

        CreateMap<MetrajRuleTemplate, GetListMetrajRuleTemplateListItemDto>();
        CreateMap<IPaginate<MetrajRuleTemplate>, GetListResponse<GetListMetrajRuleTemplateListItemDto>>();

        CreateMap<MetrajRuleTemplate, GetListByDynamicMetrajRuleTemplateListItemDto>();
        CreateMap<IPaginate<MetrajRuleTemplate>, GetListResponse<GetListByDynamicMetrajRuleTemplateListItemDto>>();
    }
}