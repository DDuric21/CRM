using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class AssetController : Controller
    {
        private readonly ICrmRepository _repository;

        public AssetController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }

        [HttpGet]
        [Route("/Assets")]
        public IEnumerable<Asset> GetAll()
        {
            var assets = _repository.Assets.GetAllAsync();

            return assets.Result;
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
    }
}
