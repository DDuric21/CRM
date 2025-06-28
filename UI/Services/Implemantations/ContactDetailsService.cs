using Models.DTO;
using Models.Requests;
using Models.Responses;
using Resources.Translations;
using UI.Helpers;

namespace UI.Services
{
    public class ContactDetailsService : IContactDetailsService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "ContactDetails";

        public ContactDetailsService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<ActionResult<object>> UpdateContactDetailsAsync(long customerId, CustomerContactDetails customerContactDetails)
        {
            var requestBody = new EditContactDetailsRQ { CustomerId = customerId, CustomerContactDetails = customerContactDetails };

            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, ApiUrl, requestBody);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);

                if (!response.IsSuccess)
                {
                    return new ActionResult<object>(Translation.ContactDetailsNotSaved);
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
