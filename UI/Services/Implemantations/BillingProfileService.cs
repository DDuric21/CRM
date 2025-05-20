using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class BillingProfileService : IBillingProfileService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public BillingProfileService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
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
                _loggingService.SendErrorLogToServerAsync(ex);
                return new BillingProfileDTO();
            }
        }


        public async Task<bool> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO)
        {
            var url = "BillingProfiles";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, billingProfileDTO);

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

        public async Task<bool> DeactivateBillingProfileAsync(string billingProfileId)
        {
            var url = $"BillingProfiles/{billingProfileId}";
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
    }
}
