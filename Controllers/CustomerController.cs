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
                //add loging
                return new Customer();
            }

            return customer.Result;
        }

        [HttpPost]
        [Route("/Customers")]
        public int InsertCustomer(Customer customer)
        {
            try
            {
                _repository.Customers.Insert(customer);

                return _repository.Customers.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }

        [HttpDelete]
        [Route("/Customers/{id}")]
        public string DeleteCustomer(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                isDeleted = _repository.Customers.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return isDeleted;
        }

        [HttpPut]
        [Route("/Customers")]
        public int UpdateCustomer(Customer customer)
        {
            try
            {
                return _repository.Customers.UpdateAsync(customer).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }
    }
}
