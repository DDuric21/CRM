using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class AddressService : IAddressService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "Addresses";

        public AddressService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<bool> UpdateAddressesAsync(List<AddressDTO> addresses)
        { 
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, ApiUrl, addresses);

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

        public async Task<AddressDTO> CreateNewAddressAsync(AddressDTO addressDTO)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, addressDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<AddressDTO>(request);
                
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new AddressDTO();
            }
        }

        public async Task<int> DeleteAddressAsync(long addressId)
        {
            var url = $"{ApiUrl}/{addressId}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<int>(request);
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return 0;
            }
        }
    }
}
