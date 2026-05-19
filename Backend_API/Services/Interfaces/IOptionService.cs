using Models.DTO;

namespace Backend_API.Services
{
    public interface IOptionService
    {
        Task<IEnumerable<OptionDTO>> GetAssetAvailableOptionsAsync(long assetID);
    }
}
