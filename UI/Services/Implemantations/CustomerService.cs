using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public CustomerService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync()
        {
            var request = _communicationService.CreateRequest(HttpMethod.Get, "https://localhost:7076/Customers");            

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<CustomerDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<CustomerDTO> GetCustomerDataAsync(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/{0}", customerID);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<CustomerDTO>(request);

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
            var request = _communicationService.CreateRequest(HttpMethod.Post, "https://localhost:7076/Customers", customerDTO);
            
            try
            {
                var response = await _communicationService.SendRequestAsync<long>(request);
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
            var request = _communicationService.CreateRequest(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
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
            var request = _communicationService.CreateRequest(HttpMethod.Put, url, customerDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

        public async Task<IAsyncEnumerable<AssetDTO>> GetCustomerAssetsAsync(long id)
        {
            var url = string.Format("https://localhost:7076/Customers/Assets");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, id);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<AssetDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<AssetDTO> GetCustomerAssetDataAsync(long customerAssetid)
        {
            var url = string.Format("https://localhost:7076/Customers/Assets/{0}", customerAssetid);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<AssetDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<IAsyncEnumerable<OrderDTO>> GetCustomerOrdersAsync(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/Orders/{0}", customerID);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<OrderDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<IAsyncEnumerable<InteractionDTO>> GetCustomerInteractionsAsync(long customerID)
        {
            var url = string.Format("https://localhost:7076/Customers/Interactions/{0}", customerID);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<InteractionDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
