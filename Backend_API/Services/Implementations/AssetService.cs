using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Enums;
using Models.Helpers;

namespace Backend_API.Services
{
    public class AssetService : IAssetService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public AssetService(
            ICrmRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<Asset> GetAssetsWithOptions()
        {
            var assets = _repository.Assets
                .With(x => x.Options)
                .ToList();

            return assets;
        }

        public IEnumerable<Option> GetAssetOptions(long customerAssetID)
        {
            var options = _repository.CustomerAssetOptions
                .Where(x => x.CustomerAssetsID == customerAssetID)
                .Select(x => x.Option);

            return options;
        }

        public IEnumerable<AssetDTO> MapCustomerAssetsToAssetDTOs(IEnumerable<CustomerAssets> customerAssets)
        {
            if (customerAssets.IsNullOrEmpty())
            {
                return new List<AssetDTO>();
            }

            try
            {
                var assetDTOs = _mapper.Map<IEnumerable<AssetDTO>>(customerAssets);

                return assetDTOs;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return new List<AssetDTO>();
            }
        }

        public AssetDTO MapAssetToDTO(Asset asset, long customerAssetsID = 0)
        {
            if (asset.IsNullOrEmpty())
            {
                return new AssetDTO();
            }

            var assetDTO = _mapper.Map<AssetDTO>(asset);
            assetDTO.Options = new List<OptionDTO>();

            foreach (var option in asset.Options ?? Enumerable.Empty<Option>())
            {
                assetDTO.Options.Add(_mapper.Map<OptionDTO>(option));
            }

            assetDTO.CustomerAssetID = customerAssetsID;

            return assetDTO;
        }

        public Asset MapDtoToAsset(AssetDTO assetDTO)
        {
            if (assetDTO.IsNullOrEmpty())
            {
                return new Asset();
            }

            var asset = _mapper.Map<Asset>(assetDTO);

            return asset;
        }

        //timePeriod filtering TODO! Curently no way of knowing when state was changed
        public async Task<Dictionary<ItemState, int>> GetAssetsChartDataAsync(DateTime timePeriod = new DateTime())
        {
            var assetStatisticsData = await _repository.CustomerAssets
                .Where(x => x.DateCreated >= timePeriod)
                .GroupBy(x => x.AssetStatusID)
                .Select(g => new                
                {
                    AssetStatusID = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = assetStatisticsData.ToDictionary(x => (ItemState)x.AssetStatusID, x => x.Count);

            return result;
        }

        public async Task<IEnumerable<AssetDTO>> GetAllAssetsAsync()
        {
            var assets = await _repository.Assets.GetAllAsync();
            if (assets.IsNullOrEmpty())
            {
                return new List<AssetDTO>();
            }

            var assetDTOs = _mapper.Map<List<AssetDTO>>(assets);

            return assetDTOs;
        }
    }
}
