using AutoMapper;
using Backend_API.Data.Model;
using Models.DTO;

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
        }
    }
}
