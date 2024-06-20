using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;

namespace Backend_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(
            ICrmRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task CreateOrderAssetAsync(CustomerAssets customerAsset)
        {
            await _repository.CustomerAssets.InsertAsync(customerAsset);
        }

        public async Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions)
        {
            await _repository.CustomerAssetOptions.InsertRange(customerAssetOptions);
        }

        public async Task<int> DeleteCustomerAssetAsync(long customerAssetsID)
        {
            return await _repository.CustomerAssets.DeleteByIdAsync(customerAssetsID);
        }

        public CustomerAssets MapToCustomerAsset(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id; 

            return customerAsset;
        }

        public List<CustomerAssetOptions> MapToCustomerAssetOptions(OrderDTO orderDTO, CustomerAssets customerAsset)
        {
            var customerAssetOptions = new List<CustomerAssetOptions>();
            foreach (var optionDTO in orderDTO.AssetDTO.Options)
            {
                var assetOption = new CustomerAssetOptions
                {
                    CustomerAssetsID = customerAsset.Id,
                    OptionID = optionDTO.Id
                };

                customerAssetOptions.Add(assetOption);
            }

            return customerAssetOptions;
        }
    }
}
