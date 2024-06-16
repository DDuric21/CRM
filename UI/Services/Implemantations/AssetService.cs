using Models.DTO;

namespace UI.Services
{
    public class AssetService : IAssetService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public AssetService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<IAsyncEnumerable<AssetDTO>> GetAssetsAsync(bool withOptions = false)
        {
            var url = string.Format("https://localhost:7076/Assets{0}", $"?withOptions={withOptions}");
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<AssetDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
