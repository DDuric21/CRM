using Models.DTO;
using Models.Helpers;

namespace Backend_API.Services
{
    public class DataValidationService : IDataValidationService
    {
        public bool ValidateCustomerDTO(CustomerDTO customerDTO)
        {
            if (customerDTO.IsNullOrEmpty())
            {
                return false;
            }

            if (customerDTO.Birthday == null
                || customerDTO.Birthday <= DateTime.MinValue)
            {
                return false;
            }

            if (string.IsNullOrEmpty(customerDTO.FirstName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(customerDTO.LastName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(customerDTO.PersonalID))
            {
                return false;
            }

            return true;
        }

        public bool ValidateAddressDTO(AddressDTO addressDTO)
        {
            if (addressDTO.IsNullOrEmpty())
            {
                return false;
            }

            if (addressDTO.CustomerId == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(addressDTO.FullAddress)) 
            { 
                return false; 
            }

            return true;
        }
    }
}
