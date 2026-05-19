using Backend_API.Data.Models;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IAddressService
    {
        /// <summary>
        /// Maps property values from model to data transfer object
        /// </summary>
        /// <param name="address">Address object</param>
        /// <returns>AddressDTO object</returns>
        AddressDTO MapAddressToDTO(Address address);

        Address MapDtoToAddress(AddressDTO addressDTO);

        Task<int> UpdateAddressesAsync(List<Address> addresses);

        Task<AddressDTO> InsertAddressAsync(AddressDTO addressDTO);
    }
}
