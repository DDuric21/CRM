using Backend_API.Data.Model;
using Models.DTO;
using System.Collections.Generic;

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
        /// Retrives all data related to customer
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>Customer object</returns>
        Task<Customer> GetCustomerDataAsync(long id);

        IEnumerable<CustomerAssets> GetCustomerAssets(long id);

        Asset GetCustomerAssetData(long customerAssetsID);

        List<Order> GetCustomerOrders(long customerID);

        List<Interaction> GetCustomerInteractions(long customerID);
    }
}
