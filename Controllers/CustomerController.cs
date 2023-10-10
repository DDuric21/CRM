using Backend_API.Data.DbContext;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICrmRepository _repository;

        public CustomerController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }

        [HttpGet]
        [Route("/Customer")]
        public IEnumerable<Customer> GetAll()
        {
            var customer = _repository.Customers.GetAllAsync();

            return customer.Result;
        }
    }
}
