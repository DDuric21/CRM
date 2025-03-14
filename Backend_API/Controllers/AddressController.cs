using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Responses;

namespace Backend_API.Controllers
{
    public class AddressController : Controller
    {
        private readonly ICrmRepository _repository;
        private readonly IAddressService _addressService;
        private readonly IDataValidationService _dataValidationService;

        public AddressController(
            ICrmRepository crmRepository,
            IAddressService addressService,
            IDataValidationService dtaValidationService)
        {
            _repository = crmRepository;
            _addressService = addressService;
            _dataValidationService = dtaValidationService;
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
        public async Task<IActionResult> InsertAddress([FromBody] AddressDTO addressDTO)
        {
            if (!_dataValidationService.ValidateAddressDTO(addressDTO))
            {
                return BadRequest();
            }

            try
            {
                var result = await _addressService.InsertAddressAsync(addressDTO);

                if (result.IsNullOrEmpty())
                {
                    return Problem("Address not inserted!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("/Addresses/{id}")]
        public async Task<IActionResult> DeleteAddress(long id)
        {
            try
            {
                var isDeleted = await _repository.Addresses.DeleteByIdAsync(id);

                if (isDeleted == 0)
                {
                    return Problem("Address not deleted!");
                }

                return Ok(isDeleted);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("/Addresses/{id}")]
        public int UpdateAddress(Address address)
        {
            try
            {
                return _repository.Addresses.UpdateAsync(address).Result;
            }
            catch (Exception ex)
            {
                //add logging
            }

            return 0;
        }

        [HttpPut]
        [Route("/Addresses")]
        public async Task<IActionResult> UpdateAddresses([FromBody] List<AddressDTO> addressDTOs)
        {
            try
            {
                var addresses = new List<Address>();
                foreach (var addressDTO in addressDTOs)
                {
                    addresses.Add(_addressService.MapDtoToAddress(addressDTO));
                }

                var result = await _addressService.UpdateAddressesAsync(addresses);

                if (result == 0)
                {
                    Problem("No entities updated!");
                }
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }

            return Ok(new ResponseBase());
        }
    }
}
