using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class AddressService : IAddressService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public AddressService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task UpdateAddressesAsync(List<AddressDTO> addresses)
        {
            var url = "Addresses";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, addresses);

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
                // logging
                _modalService.ShowErrorMessage(ex.Message);
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
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return 0;
            }
        }
    }
}
