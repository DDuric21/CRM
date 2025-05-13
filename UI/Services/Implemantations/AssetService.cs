using Models.DTO;
using Models.Enums;

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
            var url = $"Assets?withOptions={withOptions}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

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

        public async Task<IEnumerable<OptionDTO>> GetAssetAvailableOptionsAsync(long assetID)
        {
            var url = $"Options/AvailableOptions/{assetID}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);
            try
            {
                var response = await _communicationService.SendRequestAsync<IEnumerable<OptionDTO>>(request);
                return response;
            }
            catch (Exception ex)
            {
                //add logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }


        public async Task<Dictionary<ItemState, int>> GetAssetsChartDataAsync()
        {
            var url = "Assets/ChartData";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<Dictionary<ItemState, int>>(request);

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
