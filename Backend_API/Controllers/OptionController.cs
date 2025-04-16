using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    [Route("Options")]
    public class OptionController : AuthorizationController
    {
        private readonly ICrmRepository _repository;

        public OptionController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }


        [HttpGet]
        public IEnumerable<Option> GetAll()
        {
            var options = _repository.Options.GetAllAsync();

            return options.Result;
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
                //add logging
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
                //add logging
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
                //add logging
            }

            return 0;
        }
    }
}
