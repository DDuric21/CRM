using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.HelperMethods;

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

        public IEnumerable<AssetDTO> MapAssetsToDTOs(IEnumerable<Asset> assets)
        {
            var assetDTOs = new List<AssetDTO>();

            if (assets.IsNullOrEmpty())
            {
                return assetDTOs;
            }

            try
            {
                foreach (var asset in assets)
                {
                    assetDTOs.Add(_mapper.Map<AssetDTO>(asset));
                }

                return assetDTOs;
            }
            catch (Exception ex)
            {
                //add loging
                return new List<AssetDTO>();
            }
        }

        public AssetDTO MapAssetToDTO(Asset asset)
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
    }
}
