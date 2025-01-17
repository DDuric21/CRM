using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Model;
using Models.DTO;
using Models.Enums;

namespace Backend_API.Data.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDTO>()
                .ForPath(dest => dest.Type, opt => opt.MapFrom(src => (ItemState)src.TypeID));
            CreateMap<CustomerDTO, Customer>()
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
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email));
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail));
            CreateMap<UserData, UserDTO>()
                .ForPath(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
