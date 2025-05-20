using Models.DTO;
using Models.Enums;

namespace UI.Services
{
    public class AssetService : IAssetService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public AssetService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
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
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<AssetDTO>();
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
                _loggingService.SendErrorLogToServerAsync(ex);
                return new List<OptionDTO>();
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
                _loggingService.SendErrorLogToServerAsync(ex);
                return new Dictionary<ItemState, int>();
            }
        }
    }
}
