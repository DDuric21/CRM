using Models.DTO;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "Interactions";

        public InteractionService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<ActionResult<CreateInteractionRS>> SaveNewInteractionAsync(InteractionDTO interactionDTO)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, interactionDTO);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<CreateInteractionRS>(request);

                if (response == null || response.InteractionId <= 0)
                {
                    return new ActionResult<CreateInteractionRS>("Interaction not created!");
                }

                return new ActionResult<CreateInteractionRS>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<CreateInteractionRS>(ex.Message);
            }
        }

        public async Task<ResponseBase> UpdateInteractionAsync(InteractionDTO interactionDTO)
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, ApiUrl, interactionDTO);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ResponseBase { ErrorMessage = ex.Message };
            }
        }
    }
}
