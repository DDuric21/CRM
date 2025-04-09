using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class BillingProfileService : IBillingProfileService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public BillingProfileService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<BillingProfileDTO> CreateNewBillingProfileAsync(long customerID)
        {
            var url = "BillingProfiles";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, customerID);

            try
            {
                var response = await _communicationService.SendRequestAsync<BillingProfileDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return new BillingProfileDTO();
            }
        }


        public async Task UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO)
        {
            var url = "BillingProfiles";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, billingProfileDTO);

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

        public async Task DeactivateBillingProfileAsync(string billingProfileId)
        {
            var url = $"BillingProfiles/{billingProfileId}";
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
    }
}
