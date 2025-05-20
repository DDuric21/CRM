using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public CustomerService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync(CustomerFilterRQ customerFilter)
        {
            var url = "Customers";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, customerFilter);          

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<CustomerDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<CustomerDTO>();
            }
        }

        public async Task<CustomerDTO> GetCustomerDataAsync(long customerID)
        {
            var url = $"Customers/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<CustomerDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new CustomerDTO();
            }
        }

        public async Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO)
        {
            var url = "Customers/Create";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, customerDTO);
            
            try
            {
                var response = await _communicationService.SendRequestAsync<long>(request);
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return 0;
            }
        }

        public async Task<bool> DeleteCustomer(long customerID)
        {
            var url = "Customers/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return false;
            }
        }

        public async Task<bool> UpdateCustomer(CustomerDTO customerDTO)
        {
            var url = "Customers";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, customerDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return false;
            }
        }

        public async Task<IAsyncEnumerable<AssetDTO>> GetCustomerAssetsAsync(long id)
        {
            var url = "Customers/Assets";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, id);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<AssetDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<AssetDTO>();
            }
        }

        public async Task<AssetDTO> GetCustomerAssetDataAsync(long customerAssetid)
        {
            var url = $"Customers/Assets/{customerAssetid}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<AssetDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new AssetDTO();
            }
        }

        public async Task<IAsyncEnumerable<OrderDTO>> GetCustomerOrdersAsync(long customerID)
        {
            var url = $"Customers/Orders/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<OrderDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<OrderDTO>();
            }
        }

        public async Task<IAsyncEnumerable<InteractionDTO>> GetCustomerInteractionsAsync(long customerID)
        {
            var url = $"Customers/Interactions/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<InteractionDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<InteractionDTO>();
            }
        }

        public async Task<CustomerGridFilterDataRS> GetCustomerFilterBaseValues()
        {
            var url = "Customers/GridFilterData";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<CustomerGridFilterDataRS>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new CustomerGridFilterDataRS();
            }
        }
    }
}
