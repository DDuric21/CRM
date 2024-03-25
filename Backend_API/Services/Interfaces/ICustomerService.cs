using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Maps property values from model to data transfer object
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>CustomerDTO object</returns>
        CustomerDTO MapCustomerToDTO(Customer customer);
    }
}
