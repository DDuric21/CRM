using Backend_API.Data.Models;
using Models.DTO;
using Models.Requests;
using Models.Responses;

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

        /// <summary>
        /// Maps property values from DTO to entity
        /// </summary>
        /// <param name="customerDTO">Customer data transfer object</param>
        /// <returns>Customer object</returns>
        Customer MapDtoToCustomer(CustomerDTO customerDTO);

        /// <summary>
        /// Retrieves all data related to customer
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>CustomerDTO object</returns>
        Task<CustomerDTO> GetCustomerDataAsync(long id);

        IEnumerable<CustomerAssets> GetCustomerAssets(long id);

        Task<CustomerAssets> GetCustomerAssetDataAsync(long customerAssetsID);

        Task<List<Order>> GetCustomerOrdersAsync(long customerID);

        List<Interaction> GetCustomerInteractions(long customerID);

        CustomerGridFilterDataRS GetCustomerFilterBaseValues();

        Task<List<Customer>> GetCustomersAsync(CustomerFilterRQ customerFilter);

        Task<CustomerContactDetails> GetCustomerContactDetailsAsync(long customerId);
    }
}
