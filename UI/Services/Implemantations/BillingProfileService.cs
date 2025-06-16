using Models.DTO;
using Models.Responses;
using Resources.Translations;
using UI.Helpers;

namespace UI.Services
{
    public class BillingProfileService : IBillingProfileService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "BillingProfiles";

        public BillingProfileService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<ActionResult<BillingProfileDTO>> CreateNewBillingProfileAsync(long customerID)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, customerID);

            try
            {
                var response = await _communicationService.SendRequestAsync<CreateNewBillingProfileRS>(request);
                if (response == null || !response.IsSuccess)
                {
                    return new ActionResult<BillingProfileDTO>(response?.ErrorMessage ?? Translation.CustomerFriendlyMessage);
                }

                return new ActionResult<BillingProfileDTO>(response.BillingProfile);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<BillingProfileDTO>(ex.Message);
            }
        }

        public async Task<ActionResult<BillingProfileDTO>> UpdateBillingProfileAsync(BillingProfileDTO billingProfileDTO)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, ApiUrl, billingProfileDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<UpdateBillingProfileRS>(request);
                if (!response.IsSuccess)
                {
                    return new ActionResult<BillingProfileDTO>(response.ErrorMessage ?? Translation.CustomerFriendlyMessage);
                }

                return new ActionResult<BillingProfileDTO>(response.BillingProfileDTO);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<BillingProfileDTO>(ex.Message);
            }
        }

        public async Task<ActionResult<object>> DeactivateBillingProfileAsync(string billingProfileId)
        {
            var url = $"{ApiUrl}/{billingProfileId}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                if (!response.IsSuccess)
                {
                    return new ActionResult<object>(false, response.ErrorMessage ?? Translation.CustomerFriendlyMessage);
                }

                return new ActionResult<object>(true);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<object>(ex.Message);
            }
        }
    }
}
