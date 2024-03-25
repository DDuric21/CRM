using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{ 
    public class AddressService : IAddressService
    {
        public AddressDTO MapAddressToDTO(Address address)
        {
            var addressDTO = new AddressDTO
            {
                FullAddress = address?.FullAddress
            };

            return addressDTO;
        }
    }
}
