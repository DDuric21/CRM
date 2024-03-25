using Backend_API.Data.Model;
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
    }
}
