using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost]
        [Route("/Addresses")]
        public int InsertAddress(Address address)
        {
            try
            {
                _repository.Addresses.InsertAsync(address);

                return _repository.Addresses.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }

        [HttpDelete]
        [Route("/Addresses/{id}")]
        public string DeleteAddress(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                isDeleted = _repository.Addresses.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return isDeleted;
        }

        [HttpPut]
        [Route("/Addresses")]
        public int UpdateAddress(Address address)
        {
            try
            {
                return _repository.Addresses.UpdateAsync(address).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }
    }
}
