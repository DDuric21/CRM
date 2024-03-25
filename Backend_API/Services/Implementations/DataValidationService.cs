using Models.DTO;
using Models.HelperMethods;

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

            if (customerDTO.Birthday.IsNullOrEmpty()
                || customerDTO.Birthday == DateTime.MinValue)
            {
                return false;
            }

            if (string.IsNullOrEmpty(customerDTO.Name))
            {
                return false;
            }

            return true;
        }
    }
}
