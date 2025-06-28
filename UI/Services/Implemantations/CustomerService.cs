using Blazored.SessionStorage;
using Models.DTO;
using Models.Requests;
using Models.Responses;
using Resources.Translations;
using UI.Authentication;
using UI.Helpers;

namespace UI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private readonly ISessionStorageService _sessionStorage;
        private const string ApiUrl = "Customers";

        public CustomerService(
            ICommunicationService communicationService,
            ILoggingService loggingService,
            ISessionStorageService sessionStorage)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
            _sessionStorage = sessionStorage;
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomersAsync(CustomerFilterRQ customerFilter)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, customerFilter);          

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

        public async Task<ActionResult<CustomerDTO>> GetCustomerDataAsync(long customerID)
        {
            try
            {
                var storageKey = $"{CrmStorageKeys.CustomerData}{customerID}";
                var customerDTO = await _sessionStorage.ReadEncryptedItemAsync<CustomerDTO>(storageKey);
                if (customerDTO != null && customerDTO.Id > 0)
                {
                    return new ActionResult<CustomerDTO>(customerDTO);
                }
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<CustomerDTO>(ex.Message);
            }

            return await GetCustomerDataFromServerAsync(customerID);
        }

        public async Task<long> CreateNewCustomerAsync(CustomerDTO customerDTO)
        {
            var url = $"{ApiUrl}/Create";
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
            var url = $"{ApiUrl}/{customerID}";
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

        public async Task<ResponseBase> UpdateCustomer(CustomerDTO customerDTO)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, ApiUrl, customerDTO);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                if (!response.IsSuccess && string.IsNullOrEmpty(response.ErrorMessage))
                {
                    response.ErrorMessage = Translation.CustomerFriendlyMessage;
                }

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<IAsyncEnumerable<AssetDTO>> GetCustomerAssetsAsync(long id)
        {
            var url = $"{ApiUrl}/Assets";
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
            var url = $"{ApiUrl}/Assets/{customerAssetid}";
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

        public async Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(long customerID)
        {
            var url = $"{ApiUrl}/Orders/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetOrdersRS>(request);

                return response.Orders;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return Enumerable.Empty<OrderDTO>();
            }
        }

        public async Task<IAsyncEnumerable<InteractionDTO>> GetCustomerInteractionsAsync(long customerID)
        {
            var url = $"{ApiUrl}/Interactions/{customerID}";
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
            var url = $"{ApiUrl}/GridFilterData";
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

        public async Task<ActionResult<CustomerContactDetails>> GetCustomerContactDetailsAsync(long customerId)
        {
            var url = $"{ApiUrl}/ContactDetails/{customerId}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<GetCustomerContactDetailsRS>(request);
                if (!response.IsSuccess)
                {
                    return new ActionResult<CustomerContactDetails>(response.ErrorMessage ?? Translation.CustomerFriendlyMessage);
                }
                else
                {
                    return new ActionResult<CustomerContactDetails>(response.CustomerContactDetails);
                }
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<CustomerContactDetails>(ex.Message);
            }
        }

        private async Task<ActionResult<CustomerDTO>> GetCustomerDataFromServerAsync(long customerID)
        {
            var url = $"{ApiUrl}/{customerID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetCustomerDataRS>(request);
                if (!response.IsSuccess)
                {
                    return new ActionResult<CustomerDTO>(response.ErrorMessage ?? Translation.CustomerFriendlyMessage);
                }

                return new ActionResult<CustomerDTO>(response.CustomerDTO);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<CustomerDTO>(ex.Message);
            }
        }
    }
}
