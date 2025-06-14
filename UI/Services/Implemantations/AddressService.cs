using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class AddressService : IAddressService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public AddressService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<bool> UpdateAddressesAsync(List<AddressDTO> addresses)
        {
            return false;   
            var url = "Addresses";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, addresses);

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
            var url = "Addresses";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, addressDTO);

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
            var url = $"Addresses/{addressId}";
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
