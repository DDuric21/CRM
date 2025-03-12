using Models.DTO;
using Models.Enums;

namespace UI.Services
{
    public interface IAssetService
    {
        Task<IAsyncEnumerable<AssetDTO>> GetAssetsAsync(bool withOptions);

        Task<Dictionary<ItemState, int>> GetAssetsChartDataAsync();
    }
}
