using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public InteractionService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<long> SaveNewInteractionAsync(InteractionDTO interactionDTO)
        {
            var url = "Interactions";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, interactionDTO);

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

        public async Task<bool> UpdateInteractionAsync(InteractionDTO interactionDTO)
        {
            var url = "Interactions";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, interactionDTO);

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
