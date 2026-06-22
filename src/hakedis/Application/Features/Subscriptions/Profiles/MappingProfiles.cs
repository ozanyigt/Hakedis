using Application.Features.Subscriptions.Commands.Create;
using Application.Features.Subscriptions.Commands.Delete;
using Application.Features.Subscriptions.Commands.Update;
using Application.Features.Subscriptions.Queries.GetById;
using Application.Features.Subscriptions.Queries.GetList;
using Application.Features.Subscriptions.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Subscriptions.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateSubscriptionCommand, Subscription>();
        CreateMap<Subscription, CreatedSubscriptionResponse>();

        CreateMap<UpdateSubscriptionCommand, Subscription>();
        CreateMap<Subscription, UpdatedSubscriptionResponse>();

        CreateMap<DeleteSubscriptionCommand, Subscription>();
        CreateMap<Subscription, DeletedSubscriptionResponse>();

        CreateMap<Subscription, GetByIdSubscriptionResponse>();

        CreateMap<Subscription, GetListSubscriptionListItemDto>();
        CreateMap<IPaginate<Subscription>, GetListResponse<GetListSubscriptionListItemDto>>();

        CreateMap<Subscription, GetListByDynamicSubscriptionListItemDto>();
        CreateMap<IPaginate<Subscription>, GetListResponse<GetListByDynamicSubscriptionListItemDto>>();
    }
}