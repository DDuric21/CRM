using Models.DTO;

namespace Backend_API.Services
{
    public interface IDataValidationService
    {
        bool ValidateCustomerDTO(CustomerDTO customerDTO);
    }
}
