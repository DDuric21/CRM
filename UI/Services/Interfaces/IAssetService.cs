using Models.DTO;

namespace UI.Services
{
    public interface IAssetService
    {
        Task<IAsyncEnumerable<AssetDTO>> GetAssetsAsync(bool withOptions);
    }
}
