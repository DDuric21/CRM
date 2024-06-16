using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace Backend_API.Controllers
{
    public class AssetController : Controller
    {
        private readonly ICrmRepository _repository;
        private readonly IAssetService _assetService;

        public AssetController(
            ICrmRepository crmRepository,
            IAssetService assetService)
        {
            _repository = crmRepository;
            _assetService = assetService;
        }

        [HttpGet]
        [Route("/Assets")]
        public async Task<IActionResult> GetAll(bool withOptions)
        {
            var assetDTOs = new List<AssetDTO>();

            try
            {
                IEnumerable<Asset> assets;
                if (withOptions)
                {
                    assets = _assetService.GetAssetsWithOptions();
                }
                else
                {
                    assets = await _repository.Assets.GetAllAsync();
                }

                foreach (var asset in assets)
                {
                    assetDTOs.Add(_assetService.MapAssetToDTO(asset));
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(assetDTOs);
        }

        [HttpGet]
        [Route("/Assets/{id}")]
        public Asset GetById(long id)
        {
            var asset = _repository.Assets.GetByIdAsync(id);

            if (asset == null)
            {
                return new Asset();
            }

            return asset.Result;
        }

        [HttpPost]
        [Route("/Assets")]
        public int InsertAsset(Asset asset)
        {
            try
            {
                _repository.Assets.InsertAsync(asset);

                return _repository.Assets.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }

        [HttpDelete]
        [Route("/Assets/{id}")]
        public string DeleteAsset(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                //isDeleted = _repository.Assets.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return isDeleted;
        }

        [HttpPut]
        [Route("/Assets")]
        public int UpdateAsset(Asset asset)
        {
            try
            {
                return _repository.Assets.UpdateAsync(asset).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }


    }
}
