using AutoMapper;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;
using Models.HelperMethods;
using Newtonsoft.Json;

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


        public async Task SubmitOrderAsync(Order order)
        {
            await _repository.Orders.UpdateAsync(order);
        }

        public async Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions)
        {
            await _repository.CustomerAssetOptions.InsertRangeAsync(customerAssetOptions);
        }

        public async Task<int> DeleteCustomerAssetAsync(long customerAssetsID)
        {
            return await _repository.CustomerAssets.DeleteByIdAsync(customerAssetsID);
        }

        public async Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets)
        {
            return await _repository.CustomerAssets.UpdateCustomerAssetDataAsync(customerAssets);
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.CustomerAssets = null;
            await _repository.Orders.InsertAsync(order);
        }

        public CustomerAssets MapToCustomerAsset(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;

            return customerAsset;
        }

        public Order MapDtoToOrder(OrderDTO orderDTO)
        {
            var customerAssetData = MapToCustomerAssetData(orderDTO);
            // neede so EF wont start creating child object in DB
            var customerAssetBasicData = MapToCustomerAssetBasicData(orderDTO);

            var orderParameters = JsonConvert.SerializeObject(customerAssetData);

            var order = new Order
            {
                OrderID = orderDTO.OrderID,
                CustomerID = orderDTO.CustomerDTO.Id,
                CustomerAssetsID = customerAssetData.Id == 0
                    ? null
                    : customerAssetData.Id,
                CustomerAssets = customerAssetBasicData,
                Parameters = orderParameters
            };

            return order;
        }

        public OrderDTO MapToDTO(Order order)
        {
            var customerAssets = JsonConvert.DeserializeObject<CustomerAssets>(order.Parameters);

            var orderDTO = new OrderDTO
            {
                OrderID = order.OrderID,
                CustomerDTO = new CustomerDTO(),
                AssetDTO = new AssetDTO()
            };

            return orderDTO;
        }

        public CustomerAssets MapToCustomerAssetData(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            if (!orderDTO.AssetDTO.IsNullOrEmpty())
            {
                var assetDTO = orderDTO.AssetDTO.Clone();
                customerAsset.Asset = _mapper.Map<Asset>(assetDTO);
            }
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;
            if (!orderDTO.CustomerDTO.IsNullOrEmpty())
            {
                var customerDTO = orderDTO.CustomerDTO.Clone();
                customerAsset.Customer = _mapper.Map<Customer>(customerDTO);
            }
            customerAsset.Id = orderDTO.AssetDTO.CustomerAssetID;

            if (!orderDTO.AssetDTO.Options.IsNullOrEmpty())
            {
                customerAsset.CustomerAssetOptions = new List<CustomerAssetOptions>();

                foreach (var optionDTO in orderDTO.AssetDTO.Options)
                {
                    var assetOption = new CustomerAssetOptions
                    {
                        CustomerAssetsID = orderDTO.AssetDTO.CustomerAssetID,
                        OptionID = optionDTO.Id,
                        Option = _mapper.Map<Option>(optionDTO).Clone()
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

        private CustomerAssets MapToCustomerAssetBasicData(OrderDTO orderDTO) 
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
                        OptionID = optionDTO.Id,
                    };

                    customerAsset.CustomerAssetOptions.Add(assetOption);
                }
            }

            return customerAsset;
        }
    }
}
