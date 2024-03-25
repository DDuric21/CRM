using Models.DTO;
using System.Text.Json;

namespace UI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommunicationService _customerService;

        public CustomerService(ICommunicationService communicationService)
        {
            _customerService = communicationService;
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync()
        {
            var request = _customerService.CreateRequest(HttpMethod.Get, "https://localhost:7076/Customers");
            var response = await _customerService.SendRequestAsync<IAsyncEnumerable<CustomerDTO>>(request);

            return response;
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/{0}", customerID);
            var request = _customerService.CreateRequest(HttpMethod.Get, url);
            var response = await _customerService.SendRequestAsync<CustomerDTO>(request);

            return response;
        }

        public async Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO)
        {
            var request = _customerService.CreateRequest(HttpMethod.Post, "https://localhost:7076/Customers", customerDTO);
            
            try
            {
                var response = await _customerService.SendRequestAsync<long>(request);
                return response;
            }
            catch (Exception ex)
            {
                // loging
            }

            return 0;
        }

    }
}
