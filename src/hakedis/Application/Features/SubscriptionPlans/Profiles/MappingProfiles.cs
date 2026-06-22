using Application.Features.SubscriptionPlans.Commands.Create;
using Application.Features.SubscriptionPlans.Commands.Delete;
using Application.Features.SubscriptionPlans.Commands.Update;
using Application.Features.SubscriptionPlans.Queries.GetById;
using Application.Features.SubscriptionPlans.Queries.GetList;
using Application.Features.SubscriptionPlans.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.SubscriptionPlans.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateSubscriptionPlanCommand, SubscriptionPlan>();
        CreateMap<SubscriptionPlan, CreatedSubscriptionPlanResponse>();

        CreateMap<UpdateSubscriptionPlanCommand, SubscriptionPlan>();
        CreateMap<SubscriptionPlan, UpdatedSubscriptionPlanResponse>();

        CreateMap<DeleteSubscriptionPlanCommand, SubscriptionPlan>();
        CreateMap<SubscriptionPlan, DeletedSubscriptionPlanResponse>();

        CreateMap<SubscriptionPlan, GetByIdSubscriptionPlanResponse>();

        CreateMap<SubscriptionPlan, GetListSubscriptionPlanListItemDto>();
        CreateMap<IPaginate<SubscriptionPlan>, GetListResponse<GetListSubscriptionPlanListItemDto>>();

        CreateMap<SubscriptionPlan, GetListByDynamicSubscriptionPlanListItemDto>();
        CreateMap<IPaginate<SubscriptionPlan>, GetListResponse<GetListByDynamicSubscriptionPlanListItemDto>>();
    }
}