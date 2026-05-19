using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Controllers
{
    [Route("Assets")]
    public class AssetController : AuthorizationController
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
        [Route("{id}")]
        public Asset GetById(long id)
        {
            var asset = _repository.Assets.GetByIdAsync(id);

            if (asset == null)
            {
                return new Asset();
            }

            return asset.Result;
        }


        [HttpGet]
        [Route("ChartData")]
        public async Task<IActionResult> GetAssetsChartData()
        {
            try
            {
                var assets = await _assetService.GetAssetsChartDataAsync();

                if (assets.IsNullOrEmpty())
                {
                    return Problem("No assets chart data found");
                }

                return Ok(assets);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public int InsertAsset(Asset asset)
        {
            try
            {
                _repository.Assets.InsertAsync(asset);

                return _repository.Assets.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return 0;
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public string DeleteAsset(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                //isDeleted = _repository.Assets.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return isDeleted;
        }

        [HttpPut]
        public int UpdateAsset(Asset asset)
        {
            try
            {
                return _repository.Assets.UpdateAsync(asset).Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return 0;
        }
    }
}
