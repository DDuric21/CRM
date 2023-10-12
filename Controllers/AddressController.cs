using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class AddressController : Controller
    {
        private readonly ICrmRepository _repository;

        public AddressController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }

        [HttpGet]
        [Route("/Addresses")]
        public IEnumerable<Address> GetAll()
        {
            var addresses = _repository.Addresses.GetAllAsync();

            return addresses.Result;
        }

        [HttpGet]
        [Route("/Addresses/{id}")]
        public Address GetById(long id)
        {
            var address = _repository.Addresses.GetByIdAsync(id);

            if (address == null)
            {
                return new Address();
            }

            return address.Result;
        }
    }
}
