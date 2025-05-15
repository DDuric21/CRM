using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Controllers
{
    [Route("Options")]
    public class OptionController : AuthorizationController
    {
        private readonly ICrmRepository _repository;
        private readonly IOptionService _optionService;

        public OptionController(
            ICrmRepository crmRepository,
            IOptionService optionService)
        {
            _repository = crmRepository;
            _optionService = optionService;
        }

        [HttpGet]
        public IEnumerable<Option> GetAll()
        {
            var options = _repository.Options.GetAllAsync();

            return options.Result;
        }

        [HttpGet]
        [Route("AvailableOptions/{assetId}")]
        public async Task<IActionResult> GetAssetAvailableOptions(long assetId)
        {
            if (assetId <= 0)
            {
                return BadRequest("Invalid assetID");
            }

            try
            {
                var options = await _optionService.GetAssetAvailableOptionsAsync(assetId);

                if (options.IsNullOrEmpty())
                {
                    DynamicLogger.LogWarning($"No options found for assetID: {assetId}");
                    options = new List<OptionDTO>();
                }

                return Ok(options);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public Option GetById(long id)
        {
            var option = _repository.Options.GetByIdAsync(id);

            if (option == null)
            {
                return new Option();
            }

            return option.Result;
        }

        [HttpPost]
        public int InsertOption(Option option)
        {
            try
            {
                _repository.Options.InsertAsync(option);

                return _repository.Options.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return 0;
        }

        [HttpDelete]
        [Route("{id}")]
        public string DeleteOption(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                //isDeleted = _repository.Options.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return isDeleted;
        }

        [HttpPut]
        public int UpdateOption(Option option)
        {
            try
            {
                return _repository.Options.UpdateAsync(option).Result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return 0;
        }
    }
}
