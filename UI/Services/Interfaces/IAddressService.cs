using Models.DTO;

namespace UI.Services
{
    public interface IAddressService
    {
        Task<bool> UpdateAddressesAsync(List<AddressDTO> addresses);
        Task<AddressDTO> CreateNewAddressAsync(AddressDTO addressDTO);
        Task<int> DeleteAddressAsync(long id);
    }
}
