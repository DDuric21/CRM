using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommunicationService _customerService;
        private readonly ICrmModalService _modalService;

        public CustomerService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _customerService = communicationService;
            _modalService = modalService;
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync()
        {
            var request = _customerService.CreateRequest(HttpMethod.Get, "https://localhost:7076/Customers");            

            try
            {
                var response = await _customerService.SendRequestAsync<IAsyncEnumerable<CustomerDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/{0}", customerID);
            var request = _customerService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _customerService.SendRequestAsync<CustomerDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
            }

            return null;
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
                // logging
            }

            return 0;
        }

        public async Task DeleteCustomer(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/{0}", customerID);
            var request = _customerService.CreateRequest(HttpMethod.Delete, url);

            try
            {
                var response = await _customerService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

        public async Task UpdateCustomer(CustomerDTO customerDTO)
        {
            var url = string.Format("https://localhost:7076/Customers/");
            var request = _customerService.CreateRequest(HttpMethod.Put, url, customerDTO);

            try
            {
                var response = await _customerService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

    }
}
