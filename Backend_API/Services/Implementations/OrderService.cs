using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.HelperMethods;

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

        public async Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets)
        {
            return await _repository.CustomerAssets.UpdateCustomerAssetDataAsync(customerAssets);
        }

        public CustomerAssets MapToCustomerAsset(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id; 

            return customerAsset;
        }

        public CustomerAssets MapToCustomerAssetData(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;
            customerAsset.Id = orderDTO.AssetDTO.CustomerAssetID;

            if (!orderDTO.AssetDTO.Options.IsNullOrEmpty())
            {
                customerAsset.CustomerAssetOptions = new List<CustomerAssetOptions>();

                foreach (var optionDTO in orderDTO.AssetDTO.Options)
                {
                    var assetOption = new CustomerAssetOptions
                    {
                        CustomerAssetsID = orderDTO.AssetDTO.CustomerAssetID,
                        OptionID = optionDTO.Id
                    };

                    customerAsset.CustomerAssetOptions.Add(assetOption);
                }
            }

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
