using Application.Features.ContractItems.Commands.Create;
using Application.Features.ContractItems.Commands.Delete;
using Application.Features.ContractItems.Commands.Update;
using Application.Features.ContractItems.Queries.GetById;
using Application.Features.ContractItems.Queries.GetList;
using Application.Features.ContractItems.Queries.GetListByDynamic;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.ContractItems.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateContractItemCommand, ContractItem>();
        CreateMap<ContractItem, CreatedContractItemResponse>();

        CreateMap<UpdateContractItemCommand, ContractItem>();
        CreateMap<ContractItem, UpdatedContractItemResponse>();

        CreateMap<DeleteContractItemCommand, ContractItem>();
        CreateMap<ContractItem, DeletedContractItemResponse>();

        CreateMap<ContractItem, GetByIdContractItemResponse>();

        CreateMap<ContractItem, GetListContractItemListItemDto>();
        CreateMap<IPaginate<ContractItem>, GetListResponse<GetListContractItemListItemDto>>();

        CreateMap<ContractItem, GetListByDynamicContractItemListItemDto>();
        CreateMap<IPaginate<ContractItem>, GetListResponse<GetListByDynamicContractItemListItemDto>>();
    }
}