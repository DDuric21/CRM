using Models.DTO;
using Models.Responses;

namespace UI.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public InteractionService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<long> SaveNewInteractionAsync(InteractionDTO interactionDTO)
        {
            var url = string.Format("https://localhost:7076/Interactions/");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, interactionDTO);

            try
            {
                var response = await _communicationService.SendRequestAsync<long>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return 0;
            }
        }

        public async Task UpdateInteractionAsync(InteractionDTO interactionDTO)
        {
            var url = string.Format("https://localhost:7076/Interactions/");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, interactionDTO);

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
