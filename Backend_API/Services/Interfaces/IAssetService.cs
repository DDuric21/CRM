using Backend_API.Data.Models;
using Models.DTO;
using Models.Enums;

namespace Backend_API.Services
{
    public interface IAssetService
    {

        /// <summary>
        /// Maps property values from model to data transfer object
        /// </summary>
        /// <param name="asset">Asset object</param>
        /// <returns>AssetDTO object</returns>
        AssetDTO MapAssetToDTO(Asset asset, long customerAssetsID = 0);

        IEnumerable<Option> GetAssetOptions(long customerAssetID);

        IEnumerable<AssetDTO> MapCustomerAssetsToAssetDTOs(IEnumerable<CustomerAssets> customerAssets);

        /// <summary>
        /// Maps property values from DTO to entity
        /// </summary>
        /// <param name="assetDTO">Asset data transfer object</param>
        /// <returns>Asset object</returns>
        Asset MapDtoToAsset(AssetDTO assetDTO);

        IEnumerable<Asset> GetAssetsWithOptions();

        Task<Dictionary<ItemState, int>> GetAssetsChartDataAsync(DateTime timePeriod = new DateTime());

        Task<IEnumerable<AssetDTO>> GetAllAssetsAsync();
    }
}
