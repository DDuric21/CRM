using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IAssetService
    {

        /// <summary>
        /// Maps property values from model to data transfer object
        /// </summary>
        /// <param name="asset">Asset object</param>
        /// <returns>AssetDTO object</returns>
        AssetDTO MapAssetToDTO(Asset asset);

        IEnumerable<AssetDTO> MapAssetsToDTOs(IEnumerable<Asset> assets);

        /// <summary>
        /// Maps property values from DTO to entity
        /// </summary>
        /// <param name="assetDTO">Asset data transfer object</param>
        /// <returns>Asset object</returns>
        Asset MapDtoToAsset(AssetDTO assetDTO);

        IEnumerable<Asset> GetAssetsWithOptions();
    }
}
