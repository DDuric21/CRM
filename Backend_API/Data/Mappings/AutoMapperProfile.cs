using AutoMapper;
using Backend_API.Data.Model;
using Models.DTO;
using Models.Enums;

namespace Backend_API.Data.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<AddressDTO, Address>();
            CreateMap<Address, AddressDTO>();
            CreateMap<Asset, AssetDTO>();
            CreateMap<AssetDTO, Asset>();
            CreateMap<Option, OptionDTO>();
            CreateMap<OptionDTO, Option>();
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();
            CreateMap<BillingProfile, BillingProfileDTO>() // treba testirati, nezz da li radi
                .AfterMap((src, dest, context) => context.Mapper.Map<Address, AddressDTO>(src.Address));
            CreateMap<BillingProfileDTO, BillingProfile>()
                .ForMember(x => x.AddressID, y => y.MapFrom(z => z.BillingAddress.Id))
                .AfterMap((src, dest) =>
                {
                    dest.AddressID = src.BillingAddress.Id == 0
                        ? null
                        : src.BillingAddress.Id;
                });
            CreateMap<Interaction, InteractionDTO>()
                .ForPath(dest => dest.Type, opt => opt.MapFrom(src => (InteractionType)src.TypeID));
            CreateMap<InteractionDTO, Interaction>()
                .ForPath(dest => dest.TypeID, opt => opt.MapFrom(src => (int)src.Type));
        }
    }
}
