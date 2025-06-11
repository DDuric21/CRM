using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.EntityFrameworkCore;
using Models.Authentication;
using Models.Classes;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using Newtonsoft.Json;
using Resources.Translations.API;

namespace Backend_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAssetService _assetService;
        private readonly IAuthenticationService _authenticationService;

        public OrderService(
            ICrmRepository repository,
            IMapper mapper,
            IAssetService assetService,
            IAuthenticationService authenticationService)
        {
            _repository = repository;
            _mapper = mapper;
            _assetService = assetService;
            _authenticationService = authenticationService;
        }

        public OrderDTO GetOrderData(Guid id)
        {
            var order = _repository.Orders
                .Where(x => x.OrderID == id)
                .Include(x => x.Customer)
                .ThenInclude(y => y.BillingProfiles)
                .FirstOrDefault();

            if (order is null)
            {
                DynamicLogger.LogError($"No order found for ID: {id}");
                return new OrderDTO();
            }

            var customerAssetsData = JsonConvert.DeserializeObject<CustomerAssets>(order.Parameters);

            order.CustomerAssets = customerAssetsData;

            var orderDTO = _mapper.Map<OrderDTO>(order);

            return orderDTO;
        }

        public async Task<ResponseBase> CreateNewOrderAsync(CreateOrderRQ createOrderRQ)
        {
            createOrderRQ.OrderDTO.OrderStatus = OrderStatus.Open;

            var validationResult = await CheckIfOrderCanBeCreatedAsync(createOrderRQ.OrderDTO);
            if (!validationResult.IsValid)
            {
                return new ResponseBase(false, validationResult.ErrorMessage);
            }

            try
            {
                var order = MapDtoToOrder(createOrderRQ.OrderDTO, createOrderRQ.WithOptions);
                await CreateOrderAsync(order);

                return new ResponseBase(true);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<bool> SubmitOrderDataAsync(OrderDTO orderDTO)
        {
            orderDTO.Action = DefineOrderAction(orderDTO.Action);

            try
            {
                var order = MapDtoToOrder(orderDTO);
                return await SubmitOrderAsync(order);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return false;
            } 
        }

        public async Task<ResponseBase> CancelOrderAsync(CancelOrderRQ cancelOrderRQ)
        {
            var validationResult = await _authenticationService.IsUserActionPermitted(cancelOrderRQ.Username, CrmPermissionNames.CancelOrder);
            if (!validationResult.IsValid) 
            {
                var errorMessage = string.Format(APITranslations.UserNotPermitted, cancelOrderRQ.Username);
                DynamicLogger.LogError(errorMessage);
                return new ResponseBase(false, errorMessage);
            }

            var order = await _repository.Orders
                .Where(x => x.OrderID == cancelOrderRQ.OrderId)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                DynamicLogger.LogError($"No order found for ID: {cancelOrderRQ.OrderId}");
                return new ResponseBase(false, APITranslations.OrderNotFound);
            }

            var result = await UpdateOrderStatusAsync(order, (int)OrderStatus.Cancelled);
            if (result <= 0)
            {
                DynamicLogger.LogError($"Failed to update order status for ID: {cancelOrderRQ.OrderId}");
                return new ResponseBase(false, APITranslations.OrderCancelationFailed);
            }

            return new ResponseBase(true);
        }

        private async Task<bool> SubmitOrderAsync(Order order)
        {
            if (order.DateSubmited == DateTime.MinValue)
            {
                order.DateSubmited = DateTime.UtcNow;
            }

            var isSuccess = await HandleOrderActionAsync(order);

            if (isSuccess)
            {
                try
                {
                    //for some reason this doesn't work if not awaited
                    await UpdateOrderStatusAsync(order, (int)OrderStatus.Finished);
                }
                catch (Exception ex)
                {
                    DynamicLogger.LogException(ex, "Error while updating order status to finished");
                }
            }

            return isSuccess;
        }

        private async Task<bool> HandleOrderActionAsync(Order order)
        {
            int result = 0;
            switch ((OrderAction)order.ActionID)
            {
                case OrderAction.Create:
                    result = await InsertNewCustomerAssetAsync(order);
                    break;
                case OrderAction.Delete:
                    result = await DeactivateCustomerAssetAsync(order);
                    break;
                case OrderAction.Update:
                    result = await UpdateCustomerAssetAsync(order.CustomerAssets, order.OrderID);
                    break;
                default:
                    DynamicLogger.LogError($"Unhandled order action used {order.ActionID}");
                    break;
            }

            var isSuccess = result > 0;
            return isSuccess;
        }

        private async Task<int> DeactivateCustomerAssetAsync(Order order)
        {
            if (!order.CustomerAssetsID.HasValue)
            {
                DynamicLogger.LogError("Order does not contain associated asset ID");
                return 0;
            }

            var result = await UpdateCustomerAssetStatusAsync(order.CustomerAssetsID.Value, (int)ItemState.Inactive);

            return result;
        }

        private async Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets, Guid orderID)
        {
            foreach (var option in customerAssets.CustomerAssetOptions)
            {
                option.CustomerAssetsID = customerAssets.Id;
            }

            var result = await _repository.CustomerAssets.UpdateCustomerAssetDataAsync(customerAssets);

            return result;
        }

        private async Task<int> UpdateOrderStatusAsync(Order order, int orderStatusID)
        {
            order.OrderStatusID = orderStatusID;
            return await _repository.Orders.PartialUpdateAsync(order, x => x.OrderStatusID);
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
            order.CustomerAssets = MapToCustomerAssetBasicData(order);

            var isSuccess = await _repository.Orders.UpdateAsync(order);

            return isSuccess;
        }

        private int DefineAssetStatus(OrderAction orderAction, ItemState? assetStatus = null)
        {
            if (assetStatus != null
                && (int)assetStatus > 0)
            {
                return (int)assetStatus;
            }

            var status = 0;
            switch (orderAction)
            {
                case OrderAction.Create:
                    status = (int)ItemState.Active;
                    break;
                case OrderAction.Update:
                    status = (int)ItemState.Active;
                    break;
                case OrderAction.Delete:
                    status = (int)ItemState.Inactive;
                    break;
                // maybe not the best idea
                default:
                    status = (int)ItemState.Active;
                    break;
            }

            return status;
        }

        private OrderAction DefineOrderAction(OrderAction crudAction)
        {
            var action = OrderAction.Create;
            if ((int)crudAction > 0)
            {
                action = crudAction;
            }

            return action;
        }

        private async Task<ValidationResult> CheckIfOrderCanBeCreatedAsync(OrderDTO orderDTO)
        {
            if (orderDTO.AssetDTO?.CustomerAssetID <= 0)
            {
                return new ValidationResult(true);
            }

            var existingOrder = await _repository.Orders
                .Where(x => x.CustomerAssetsID == orderDTO.AssetDTO.CustomerAssetID
                    && x.OrderStatusID == (int)OrderStatus.Open)
                .FirstOrDefaultAsync();

            if (existingOrder != null)
            {
                DynamicLogger.LogError($"Order {orderDTO.OrderID} can not be created as open orders still exist.");
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Open order for this asset already exists. ID: {existingOrder.OrderID}"
                };
            }

            return new ValidationResult(true);
        }

        private async Task CreateOrderAsync(Order order)
        {
            order.CustomerAssets = null;
            await _repository.Orders.InsertAsync(order);
        }

        #region Mappings

        private void MapOptionsToOrderAsset(OrderDTO orderDTO)
        {
            if (orderDTO?.AssetDTO?.CustomerAssetID <= 0)
            {
                DynamicLogger.LogError("Incorrect CustomerAssetID provided");
                throw new Exception("Incorrect CustomerAssetID provided");
            }

            var assetOptions = _assetService.GetAssetOptions(orderDTO.AssetDTO.CustomerAssetID);

            orderDTO.AssetDTO.Options = assetOptions
                .Select(x => _mapper.Map<OptionDTO>(x))
                .ToList();
        }

        private Order MapDtoToOrder(OrderDTO orderDTO, bool withOptions = false)
        {
            if (withOptions)
            {
                MapOptionsToOrderAsset(orderDTO);
            }

            var order = _mapper.Map<Order>(orderDTO);

            order.Parameters = JsonConvert.SerializeObject(order.CustomerAssets);

            return order;
        }

        public OrderDTO MapToDTO(Order order)
        {
            var customerAssets = JsonConvert.DeserializeObject<CustomerAssets>(order.Parameters);

            var orderDTO = new OrderDTO
            {
                OrderID = order.OrderID,
                OrderStatus = (OrderStatus)order.OrderStatusID,
                Action = (OrderAction)order.ActionID,
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

        private CustomerAssets MapToCustomerAssetBasicData(Order order)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = order.CustomerAssets.AssetID;
            customerAsset.CustomerID = order.CustomerID;
            customerAsset.Id = order.CustomerAssets.Id;
            customerAsset.AssetStatusID = DefineAssetStatus((OrderAction)order.ActionID, (ItemState)order.CustomerAssets.AssetStatusID);
            customerAsset.BillingProfileId = order.CustomerAssets.BillingProfileId;

            if (!order.CustomerAssets.CustomerAssetOptions.IsNullOrEmpty())
            {
                customerAsset.CustomerAssetOptions = new List<CustomerAssetOptions>();

                foreach (var option in order.CustomerAssets.CustomerAssetOptions)
                {
                    var assetOption = new CustomerAssetOptions
                    {
                        CustomerAssetsID = order.CustomerAssets.Id,
                        OptionID = option.OptionID,
                    };

                    customerAsset.CustomerAssetOptions.Add(assetOption);
                }
            }

            return customerAsset;
        }

        #endregion

    }
}
