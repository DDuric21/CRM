using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Services
{ 
    public class AddressService : IAddressService
    {

        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public AddressService(
            ICrmRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AddressDTO MapAddressToDTO(Address address)
        {
            if (address.IsNullOrEmpty())
            {
                return new AddressDTO();
            }

            var customerDTO = _mapper.Map<AddressDTO>(address);

            return customerDTO;
        }

        public Address MapDtoToAddress(AddressDTO addressDTO)
        {
            if (addressDTO.IsNullOrEmpty())
            {
                return new Address();
            }

            var customer = _mapper.Map<Address>(addressDTO);

            return customer;
        }

        public async Task<int> UpdateAddressesAsync(List<Address> addresses)
        {
            var addressIDs = addresses
                .Select(x => x.Id)
                .ToHashSet();

            return await _repository.Addresses.BulkUpdate(addresses, addressIDs);
        }

        public async Task<AddressDTO> InsertAddressAsync(AddressDTO addressDTO)
        {
            var address = MapDtoToAddress(addressDTO);

            try
            {
                await _repository.Addresses.InsertAsync(address);

                var createdAddress = MapAddressToDTO(address);

                return createdAddress;
            }
            catch (Exception ex)
            {
                //add logging
                return new AddressDTO();
            }
        }
    }
}
