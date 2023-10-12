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
        [Route("/Customers")]
        public IEnumerable<Customer> GetAll()
        {
            var customer = _repository.Customers.GetAllAsync();

            return customer.Result;
        }

        [HttpGet]
        [Route("/Customers/{id}")]
        public Customer GetById(long id)
        {
            var customer = _repository.Customers.GetByIdAsync(id);

            if (customer == null)
            {
                return new Customer();
            }

            return customer.Result;
        }
    }
}
