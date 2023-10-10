using Backend_API.Data.DbContext;
using Backend_API.Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CrmDbContext _context;

        public CustomerController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/Customer")]
        public IEnumerable<Customer> GetAll()
        {
            var customer = _context.Customers.ToList();

            return customer;
        }
    }
}
