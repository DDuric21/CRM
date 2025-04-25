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
            CreateMap<Customer, CustomerDTO>()
                .ForPath(dest => dest.Type, opt => opt.MapFrom(src => (ItemState)src.TypeID))
                .ForPath(dest => dest.CustomerStatus, opt => opt.MapFrom(src => (ItemState)src.CustomerStatusID));
            CreateMap<CustomerDTO, Customer>()
                .ForPath(dest => dest.CustomerStatusID, opt => opt.MapFrom(src => (int)src.CustomerStatus))
                .ForPath(dest => dest.TypeID, opt => opt.MapFrom(src => (int)src.Type));
            CreateMap<AddressDTO, Address>();
            CreateMap<Address, AddressDTO>();
            CreateMap<Asset, AssetDTO>();
            CreateMap<AssetDTO, Asset>();
            CreateMap<Option, OptionDTO>();
            CreateMap<OptionDTO, Option>();
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();
            CreateMap<BillingProfile, BillingProfileDTO>()
                .ForPath(dest => dest.BilingProfileStatus, opt => opt.MapFrom(src => (ItemState)src.BillingProfileStatusID))
                .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.Address));
            CreateMap<BillingProfileDTO, BillingProfile>()
                .ForPath(dest => dest.BillingProfileStatusID, opt => opt.MapFrom(src => (int)src.BilingProfileStatus))
                .ForMember(x => x.AddressID, y => y.MapFrom(z => z.BillingAddress.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.BillingAddress));
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
