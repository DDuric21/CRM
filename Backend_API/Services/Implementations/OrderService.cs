using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Newtonsoft.Json;

namespace Backend_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAssetService _assetService;

        public OrderService(
            ICrmRepository repository,
            IMapper mapper,
            IAssetService assetService)
        {
            _repository = repository;
            _mapper = mapper;
            _assetService = assetService;
        }

        public async Task<int> SubmitOrderAsync(Order order)
        {
            if (order.DateSubmited == DateTime.MinValue)
            {
                order.DateSubmited = DateTime.UtcNow;
            }

            int result = 0;
            switch ((CrudAction)order.ActionID)
            {
                case CrudAction.Create:
                    result = await InsertNewCustomerAssetAsync(order);
                    break;
                case CrudAction.Delete:
                    result = await DeactivateCustomerAssetAsync(order);
                    break;
                case CrudAction.Update:
                    result = await UpdateCustomerAssetAsync(order.CustomerAssets, order.OrderID);
                    break;
                default:
                    //logging
                    break;
            }

            await _repository.Orders.PartialUpdateAsync(order, x => x.DateSubmited);

            if (result > 0)
            {
                try
                { 
                    //iz nekog razloga ako se ovo ne await-a onda ne radi
                    await UpdateOrderStatusAsync(order, (int)OrderStatus.Finished);
                }
                catch (Exception ex)
                {
                    // add logging no need to allert the agent
                }
            }

            return result;
        }

        public async Task CreateOrderAssetOptionsAsync(List<CustomerAssetOptions> customerAssetOptions)
        {
            await _repository.CustomerAssetOptions.InsertRangeAsync(customerAssetOptions);
        }

        public async Task<int> DeactivateCustomerAssetAsync(Order order)
        {
            if (!order.CustomerAssetsID.HasValue)
            {
                DynamicLogger.LogError(nameof(OrderService), "Order does not contain associated asset ID");
                return 0;
            }

            var result = await UpdateCustomerAssetStatusAsync(order.CustomerAssetsID.Value, (int)ItemState.Inactive);

            return result;
        }

        // ne radi ne koristiti vidjeti kako ovo rješiti jer rješenje je elegantno
        public async Task<int> UpdateOrderStatusAsync(Guid orderID, int orderStatusID)
        {
            var order = new Order
            {
                OrderID = orderID,
                OrderStatusID = orderStatusID
            };

            return await _repository.Orders.PartialUpdateAsync(order, x => x.OrderStatusID);
        }

        public async Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets, Guid orderID)
        {
            var result = await _repository.CustomerAssets.UpdateCustomerAssetDataAsync(customerAssets);

            return result;
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.CustomerAssets = null;
            await _repository.Orders.InsertAsync(order);
        }

        public Order GetOrderData(Guid id)
        {
            var order = _repository.Orders
                .Where(x => x.OrderID == id)
                .FirstOrDefault();

            if (order is null)
            {
                return new Order();
            }

            var customerAssetsData = JsonConvert.DeserializeObject<CustomerAssets>(order.Parameters);

            order.CustomerAssets = customerAssetsData;
            order.Customer = customerAssetsData.Customer;

            return order;
        }

        public CustomerAssets MapToCustomerAsset(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;

            return customerAsset;
        }

        public void MapOptionsToOrderAsset(OrderDTO orderDTO)
        {
            if (orderDTO?.AssetDTO?.CustomerAssetID <= 0)
            {
                // add logging
                throw new Exception("Incorrect CustomerAssetID provided");
            }

            var assetOptions = _assetService.GetAssetOptions(orderDTO.AssetDTO.CustomerAssetID);

            orderDTO.AssetDTO.Options = assetOptions
                .Select(x => _mapper.Map<OptionDTO>(x))
                .ToList();
        }

        public Order MapDtoToOrder(OrderDTO orderDTO, bool withOptions = false)
        {
            if (withOptions)
            {
                MapOptionsToOrderAsset(orderDTO);
            }

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
                ActionID = (int)orderDTO.Action,
                OrderStatusID = (int)orderDTO.OrderStatus,
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
                OrderStatus = (OrderStatus)order.OrderStatusID,
                Action = (CrudAction)order.ActionID,
                AssetDTO = _mapper.Map<AssetDTO>(customerAssets.Asset),
                DateSubmited = order.DateSubmited,
                DateCreated = order.DateCreated
            };

            if (order.CustomerAssetsID.HasValue)
            {
                orderDTO.AssetDTO.CustomerAssetID = order.CustomerAssetsID.Value;
            }

            if (!order.Customer.IsNullOrEmpty())
            {
                orderDTO.CustomerDTO = _mapper.Map<CustomerDTO>(order.Customer);
            }

            if (!order.CustomerAssets.IsNullOrEmpty()
                && !order.CustomerAssets.CustomerAssetOptions.IsNullOrEmpty())
            {
                orderDTO.AssetDTO.Options = order.CustomerAssets.CustomerAssetOptions
                    .Select(x => _mapper.Map<OptionDTO>(x.Option))
                    .ToList();
            }

            return orderDTO;
        }

        public CustomerAssets MapToCustomerAssetData(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();
            customerAsset.AssetStatusID = DefineAssetStatus(orderDTO.Action, orderDTO.AssetDTO.AssetStatus);

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

        private int DefineAssetStatus(CrudAction orderAction, ItemState? assetStatus = null)
        {
            if (assetStatus != null
                && (int)assetStatus > 0)
            {
                return (int)assetStatus;
            }

            var status = 0;
            switch (orderAction)
            {
                case CrudAction.Create:
                    status = (int)ItemState.Active;
                    break;
                case CrudAction.Update:
                    status = (int)ItemState.Active;
                    break;
                case CrudAction.Delete:
                    status = (int)ItemState.Inactive;
                    break;
                // maybe not the best idea
                default:
                    status = (int)ItemState.Active;
                    break;
            }

            return status;
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

        public CustomerAssets MapToCustomerAssetBasicData(OrderDTO orderDTO) 
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;
            customerAsset.Id = orderDTO.AssetDTO.CustomerAssetID;
            customerAsset.AssetStatusID = DefineAssetStatus(orderDTO.Action, orderDTO.AssetDTO.AssetStatus);

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

        private async Task<int> UpdateCustomerAssetStatusAsync(long customerAssetID, int assetStatusID)
        {
            var customerAsset = new CustomerAssets
            {
                Id = customerAssetID,
                AssetStatusID = assetStatusID
            };

            return await _repository.CustomerAssets.PartialUpdateAsync(customerAsset, x => x.AssetStatusID);
        }

        private async Task<int> InsertNewCustomerAssetAsync(Order order)
        {
            return await _repository.Orders.UpdateAsync(order);
        }

        public CrudAction DefineOrderAction(CrudAction crudAction)
        {
            var action = CrudAction.Create;
            if ((int)crudAction > 0)
            {
                action = crudAction;
            }

            return action;
        }

        public async Task<int> UpdateOrderStatusAsync(Order order, int orderStatusID)
        {
            order.OrderStatusID = orderStatusID;
            return await _repository.Orders.PartialUpdateAsync(order, x => x.OrderStatusID);
        }
    }
}
