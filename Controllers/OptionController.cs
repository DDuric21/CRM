using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class OptionController : Controller
    {
        private readonly ICrmRepository _repository;

        public OptionController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }


        [HttpGet]
        [Route("/Options")]
        public IEnumerable<Option> GetAll()
        {
            var options = _repository.Options.GetAllAsync();

            return options.Result;
        }

        [HttpGet]
        [Route("/Options/{id}")]
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
        [Route("/Options")]
        public int InsertOption(Option option)
        {
            try
            {
                _repository.Options.Insert(option);

                return _repository.Options.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }

        [HttpDelete]
        [Route("/Options/{id}")]
        public string DeleteOption(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                isDeleted = _repository.Options.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return isDeleted;
        }

        [HttpPut]
        [Route("/Options")]
        public int UpdateOption(Option option)
        {
            try
            {
                return _repository.Options.UpdateAsync(option).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }
    }
}
