using Models.DTO;
using Models.Requests;
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
                _modalService.ShowErrorMessage(ex.Message);

                return null;
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
                //add logging
            }

            return null;
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
                // logging
            }

            return 0;
        }

        public async Task DeleteCustomer(long customerID)
        {
            var url = "Customers/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

        public async Task UpdateCustomer(CustomerDTO customerDTO)
        {
            var url = "Customers";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, customerDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
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
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
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
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
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
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
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
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
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
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
