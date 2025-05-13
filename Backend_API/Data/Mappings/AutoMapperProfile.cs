using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Models;
using Microsoft.AspNetCore.Identity;
using Models.DTO;
using Models.Enums;
using System.Security.Claims;

namespace Backend_API.Data.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ENTITY ➜ DTO

            CreateMap<Address, AddressDTO>();
            CreateMap<Option, OptionDTO>();
            CreateMap<CustomerAssetOptions, OptionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OptionID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Option.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Option.Price));
            CreateMap<Asset, AssetDTO>();
            CreateMap<BillingProfile, BillingProfileDTO>()
                .ForMember(dest => dest.BilingProfileStatus, opt => opt.MapFrom(src => (ItemState)src.BillingProfileStatusID))
                .ForMember(dest => dest.BillingProfileId, opt => opt.MapFrom(src => src.BillingProfileId))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.Address));

            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.CustomerStatus, opt => opt.MapFrom(src => (ItemState)src.CustomerStatusID))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (CustomerType)src.TypeID));

            CreateMap<CustomerAssets, AssetDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AssetID))
                .ForMember(dest => dest.CustomerAssetID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Asset.Name))
                .ForMember(dest => dest.AssetStatus, opt => opt.MapFrom(src => (ItemState)src.AssetStatusID))
                .ForMember(dest => dest.BillingProfile, opt => opt.MapFrom(src =>
                    src.BillingProfile != null
                        ? src.BillingProfile
                        : new BillingProfile(src.BillingProfileId)
                ))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Asset.Price))
                .ForMember(dest => dest.CurrencyID, opt => opt.MapFrom(src => src.Asset.CurrencyID))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.CustomerAssetOptions));

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.CustomerDTO, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.AssetDTO, opt => opt.MapFrom(src => src.CustomerAssets))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => (OrderStatus)src.OrderStatusID))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => (OrderAction)src.ActionID));

            // DTO ➜ ENTITY
            CreateMap<AddressDTO, Address>();
            CreateMap<AssetDTO, Asset>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<OptionDTO, Option>();
            CreateMap<OptionDTO, CustomerAssetOptions>()
                .ForMember(dest => dest.OptionID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Option, opt => opt.MapFrom(src => src));

            CreateMap<CustomerDTO, Customer>()
                .ForMember(dest => dest.CustomerStatusID, opt => opt.MapFrom(src => (int)src.CustomerStatus))
                .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => (int)src.Type))
                .ForMember(dest => dest.Assets, opt => opt.Ignore());

            CreateMap<BillingProfileDTO, BillingProfile>()
                .ForMember(dest => dest.BillingProfileId, opt => opt.MapFrom(src => src.BillingProfileId))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.BillingProfileStatusID, opt => opt.MapFrom(src => (int)src.BilingProfileStatus))
                .ForMember(dest => dest.AddressID, opt =>
                {
                    opt.PreCondition(src => src.BillingAddress != null);
                    opt.MapFrom(src => src.BillingAddress.Id);
                });

            CreateMap<AssetDTO, CustomerAssets>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerAssetID))
                .ForMember(dest => dest.AssetID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssetStatusID, opt => opt.MapFrom(src => (int)src.AssetStatus))
                .ForMember(dest => dest.Asset, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.BillingProfileId, opt => opt.MapFrom(src => src.BillingProfile.BillingProfileId))
                .ForMember(dest => dest.BillingProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAssetOptions, opt => opt.MapFrom(src => src.Options));

            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerDTO.Id))
                .ForMember(dest => dest.OrderStatusID, opt => opt.MapFrom(src => (int)src.OrderStatus))
                .ForMember(dest => dest.ActionID, opt => opt.MapFrom(src => (int)src.Action))
                .ForMember(dest => dest.CustomerAssetsID, opt =>
                {
                    opt.PreCondition(src => src.AssetDTO.CustomerAssetID > 0);
                    opt.MapFrom(src => src.AssetDTO.CustomerAssetID);
                })
                .ForMember(dest => dest.CustomerAssets, opt => opt.MapFrom(src => src.AssetDTO))
                .ForPath(dest => dest.CustomerAssets.CustomerID, opt => opt.MapFrom(src => src.CustomerDTO.Id))
                .ForMember(dest => dest.Customer, opt => opt.Ignore());

            CreateMap<Interaction, InteractionDTO>()
                .ForPath(dest => dest.Type, opt => opt.MapFrom(src => (InteractionType)src.TypeID));
            CreateMap<InteractionDTO, Interaction>()
                .ForPath(dest => dest.TypeID, opt => opt.MapFrom(src => (int)src.Type));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.UserStatus, opt => opt.MapFrom(src => (ItemState)src.UserStatusID));
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForPath(dest => dest.UserStatusID, opt => opt.MapFrom(src => (int)src.UserStatus));
            CreateMap<UserData, UserDTO>()
                .ForPath(
                    dest => dest.UserRoles, 
                    opt => opt.MapFrom(
                        src => src.UserRoles
                            .Select(x => new UserRoleDTO 
                            { 
                                RoleName = x.Key.Name, 
                                Permissions = x.Value.Select(y => y.Type)
                            })
                            .ToList()))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForPath(dest => dest.UserStatus, opt => opt.MapFrom(src => (ItemState)src.User.UserStatusID));
            CreateMap<IdentityRole, UserRoleDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name)); 
            CreateMap<UserDTO, UserData>()
                .ForPath(
                    dest => dest.UserRoles, 
                    opt => opt.MapFrom(
                        src => src.UserRoles.ToDictionary(
                            x => new IdentityRole(x.RoleName), 
                            x => x.Permissions
                                .Select(y => new Claim("permission", y))
                                .ToList())))
                .ForPath(dest => dest.User.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForPath(dest => dest.User.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForPath(dest => dest.User.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForPath(dest => dest.User.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForPath(dest => dest.User.UserStatusID, opt => opt.MapFrom(src => (int)src.UserStatus));
            CreateMap<News, NewsDTO>()
                .ForPath(dest => dest.NewsType, opt => opt.MapFrom(src => (NewsType)src.NewsTypeID));
        }
    }
}
