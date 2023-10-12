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
    }
}
