using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Backend_API.MessageCommands;
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
        private readonly IQueueActionRepository _queueActionRepository;
        private readonly IMapper _mapper;
        private readonly IAssetService _assetService;
        private readonly IAuthorizationService _authorizationService;
        private readonly CrmUserManager _userManager;

        public OrderService(
            ICrmRepository repository,
            IMapper mapper,
            IAssetService assetService,
            IAuthorizationService authorizationService,
            CrmUserManager userManager,
            IQueueActionRepository queueActionRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _assetService = assetService;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _queueActionRepository = queueActionRepository;
        }

        public async Task<OrderDTO> GetOrderDataAsync(Guid id)
        {
            var order = await _repository.Orders
                .Where(x => x.OrderID == id)
                .Include(x => x.CreatedByUser)
                .Include(x => x.Customer)
                .ThenInclude(y => y.BillingProfiles)
                .Include(x => x.Customer)
                .ThenInclude(y => y.Addresses)
                .FirstOrDefaultAsync();

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
            await _authorizationService.IsUserActionPermitted(createOrderRQ.Username, CrmPermissionNames.CreateOrder);

            var validationResult = await CheckIfOrderCanBeCreatedAsync(createOrderRQ.OrderDTO);
            if (!validationResult.IsValid)
            {
                return new ResponseBase(false, validationResult.ErrorMessage);
            }

            createOrderRQ.OrderDTO.OrderStatus = OrderStatus.Open;
            try
            {
                createOrderRQ.OrderDTO.CreatedByUsername = createOrderRQ.Username;
                var order = await MapDtoToOrderAsync(createOrderRQ.OrderDTO, createOrderRQ.WithOptions);
                await CreateOrderAsync(order);

                return new ResponseBase(true);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return new ResponseBase(false, ex.Message);
            }
        }

        public async Task<ResponseBase> SubmitOrderDataAsync(OrderDTO orderDTO)
        {
            orderDTO.Action = DefineOrderAction(orderDTO.Action);
            orderDTO.OrderStatus = OrderStatus.Submitted;

            try
            {
                var order = await MapDtoToOrderAsync(orderDTO);
                await SetActionIds(orderDTO);

                var result = await SubmitOrderAsync(order);

                if (result)
                {
                    await PublishChangeToBillingAsync(order, orderDTO);
                }

                return new ResponseBase(result);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return new ResponseBase(false, APITranslations.OrderSubmissionFailed);
            } 
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int orderStatusId)
        {
            var order = await _repository.Orders
                .Where(x => x.OrderID == orderId)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                DynamicLogger.LogError($"No order found for ID: {orderId}");
                return false;
            }

            var result = await UpdateOrderStatusInternalAsync(order, orderStatusId);
            if (result <= 0)
            {
                DynamicLogger.LogError($"Failed to update order status for ID: {orderId}");
                return false;
            }

            return true;
        }

        private async Task PublishChangeToBillingAsync(Order order, OrderDTO orderDTO)
        {
            var billableData = await RetrieveAssetDataAsync(order, orderDTO);

            if (!order.CustomerAssets.CustomerAssetOptions.IsNullOrEmpty())
            {
                billableData.CustomerAssetAdditions = await RetrieveAssetOptionsDataAsync(order, billableData, orderDTO);
            }

            var updateBillingCommand = new UpdateBillingCommand
            {
                OrderId = order.OrderID,
                BillingProfileId = order.CustomerAssets.BillingProfileId,
                CustomerAssetBillableData = billableData
            };

            var action = new QueueAction
            {
                Type = nameof(UpdateBillingCommand),
                Payload = JsonConvert.SerializeObject(updateBillingCommand)
            };

            await _queueActionRepository.AddAsync(action);
        }

        private async Task<IEnumerable<CustomerAssetAddition>> RetrieveAssetOptionsDataAsync(Order order, CustomerAssetBillableData billableData, OrderDTO orderDTO)
        {
            var optionIDs = orderDTO.AssetDTO.Options
                .Select(x => x.Id)
                .ToList();

            var assetOptionsData = await _repository.Options
                .Where(x => optionIDs.Contains(x.Id))
                .ToListAsync();

            var assetAdditionsData = assetOptionsData
                .Select(x => new CustomerAssetAddition
                {
                    AdditionId = x.Id,
                    Price = x.Price,
                    CurrencyID = x.CurrencyID,
                    AdditionActionId = (int)orderDTO.AssetDTO.Options.First(y => y.Id == x.Id).ItemAction,
                })
                .ToList();

            return assetAdditionsData;
        }

        private async Task SetActionIds(OrderDTO orderDto)
        {
            if (orderDto.IsNullOrEmpty())
            {
                return;
            }

            switch (orderDto.Action)
            {
                case OrderAction.CreateAsset:
                    orderDto.AssetDTO.ItemAction = ItemAction.Activate;
                    orderDto.AssetDTO.Options?.ForEach(x => x.ItemAction = ItemAction.Activate);
                    break;
                case OrderAction.UpdateAsset:
                    orderDto.AssetDTO.ItemAction = orderDto.AssetDTO.ItemAction == ItemAction.None
                        ? ItemAction.Update
                        : orderDto.AssetDTO.ItemAction;
                    await DetermineUpdateAdditionsActionsAsync(orderDto);
                    break;
                case OrderAction.DeactivateAsset:
                    orderDto.AssetDTO.ItemAction = ItemAction.Deactivate;
                    orderDto.AssetDTO.Options?.ForEach(x => x.ItemAction = ItemAction.Deactivate);
                    break;
            }
        }

        private async Task DetermineUpdateAdditionsActionsAsync(OrderDTO orderDto)
        {
            if (orderDto.AssetDTO.IsNullOrEmpty() || orderDto.AssetDTO.Options.IsNullOrEmpty())
            {
                return;
            }

            var existingOptions = await _repository.CustomerAssetOptions
                    .Where(x => x.CustomerAssetsID == orderDto.AssetDTO.CustomerAssetID)
                    .ToListAsync();

            foreach (var option in orderDto.AssetDTO.Options)
            {
                if (option.ItemAction == ItemAction.None)
                {
                    option.ItemAction = existingOptions.Any(x => x.OptionID == option.Id)
                        ? option.ItemAction
                        : ItemAction.Activate;
                }
            }

            var optionsToDeactivate = existingOptions
              .Where(x => !orderDto.AssetDTO.Options.Select(y => y.Id).Contains(x.OptionID))
                .Select(x => new OptionDTO
                {
                    ItemAction = ItemAction.Deactivate,
                    Id = x.OptionID
                })
              .ToList();

            orderDto.AssetDTO.Options.AddRange(optionsToDeactivate);
        }

        private async Task<CustomerAssetBillableData> RetrieveAssetDataAsync(Order order, OrderDTO orderDTO)
        {
            var assetData = await _repository.Assets
                .Where(x => x.Id == order.CustomerAssets.AssetID)
                .FirstOrDefaultAsync();

            var billableData = new CustomerAssetBillableData
            {
                CustomerAssetId = order.CustomerAssets.Id,
                Price = assetData?.Price ?? throw new ArgumentException($"No price found for assetID: {order.CustomerAssets.Id} !"),
                CurrencyID = assetData?.CurrencyID ?? throw new ArgumentException($"No currency ID found for assetID: {order.CustomerAssets.Id}!"),
                CustomerAssetActionId = (int)orderDTO.AssetDTO.ItemAction
            };

            return billableData;
        }

        public async Task<ResponseBase> CancelOrderAsync(CancelOrderRQ cancelOrderRQ)
        {
            await _authorizationService.IsUserActionPermitted(cancelOrderRQ.Username, CrmPermissionNames.CancelOrder);

            var order = await _repository.Orders
                .Where(x => x.OrderID == cancelOrderRQ.OrderId)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                DynamicLogger.LogError($"No order found for ID: {cancelOrderRQ.OrderId}");
                return new ResponseBase(false, APITranslations.OrderNotFound);
            }

            var result = await UpdateOrderStatusInternalAsync(order, (int)OrderStatus.Cancelled);
            if (result <= 0)
            {
                DynamicLogger.LogError($"Failed to update order status for ID: {cancelOrderRQ.OrderId}");
                return new ResponseBase(false, APITranslations.OrderCancelationFailed);
            }

            return new ResponseBase(true);
        }

        public async Task<OrderGridFilterDataRS> GetOrderFilterBaseValuesAsync()
        {
            var orderStatuses = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .ToList();

            var assets = await _assetService.GetAllAssetsAsync();
            var assetTypes = assets.ToDictionary(x => x.Id, x => x.Name);

            var response = new OrderGridFilterDataRS
            {
                OrderStatuses = orderStatuses,
                AssetTypes = assetTypes,
            };

            return response;
        }

        public async Task<GetOrdersRS> GetOrdersAsync(OrderFilter orderFilter)
        {
            var orders = _repository.Orders
                .With(x => x.CreatedByUser)
                .Include(x => x.CustomerAssets);

            var filteredOrders = await FilterOrdersAsync(orders, orderFilter);

            var orderDTOs = new List<OrderDTO>();
            foreach (var order in filteredOrders)
            {
                var orderDTO = MapToDTO(order);

                orderDTOs.Add(orderDTO);
            }

            return new GetOrdersRS { Orders = orderDTOs };
        }

        private async Task<bool> SubmitOrderAsync(Order order)
        {
            SetSubmitDate(order);

            int result = 0;
            switch ((OrderAction)order.ActionID)
            {
                case OrderAction.CreateAsset:
                    result = await InsertNewCustomerAssetAsync(order);
                    break;
                case OrderAction.UpdateAsset:
                    result = await UpdateCustomerAssetAsync(order.CustomerAssets);
                    await _repository.Orders.PartialUpdateAsync(order, x => x.DateSubmitted, x => x.OrderStatusID);
                    break;
                case OrderAction.DeactivateAsset:
                    result = await DeactivateCustomerAssetAsync(order);
                    await _repository.Orders.PartialUpdateAsync(order, x => x.DateSubmitted, x => x.OrderStatusID);
                    break;
                default:
                    DynamicLogger.LogError($"Unhandled order action used {order.ActionID}");
                    break;
            }

            var isSuccess = result > 0;
            return isSuccess;
        }

        private static void SetSubmitDate(Order order)
        {
            if (order.DateSubmitted == DateTime.MinValue)
            {
                order.DateSubmitted = DateTime.UtcNow;
            }
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

        private async Task<int> UpdateCustomerAssetAsync(CustomerAssets customerAssets)
        {
            foreach (var option in customerAssets.CustomerAssetOptions)
            {
                option.CustomerAssetsID = customerAssets.Id;
            }

            var result = await _repository.CustomerAssets.UpdateCustomerAssetDataAsync(customerAssets);

            return result;
        }

        private async Task<int> UpdateOrderStatusInternalAsync(Order order, int orderStatusID)
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
                case OrderAction.CreateAsset:
                    status = (int)ItemState.Active;
                    break;
                case OrderAction.UpdateAsset:
                    status = (int)ItemState.Active;
                    break;
                case OrderAction.DeactivateAsset:
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
            var action = OrderAction.CreateAsset;
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
                    ErrorMessage = string.Format(APITranslations.OpenOrderExists, existingOrder.OrderID)
                };
            }

            return new ValidationResult(true);
        }

        private async Task CreateOrderAsync(Order order)
        {
            order.CustomerAssets = null;
            await _repository.Orders.InsertAsync(order);
        }

        private async Task<IEnumerable<Order>> FilterOrdersAsync(IQueryable<Order> orders, OrderFilter orderFilter)
        {
            if (!string.IsNullOrEmpty(orderFilter.OrderID))
            {
                orders = orders.Where(x => x.OrderID.ToString() == orderFilter.OrderID);
            }

            if (!orderFilter.AssetTypes.IsNullOrEmpty())
            {
                orders = orders.Where(x => orderFilter.AssetTypes.Keys.Contains(x.CustomerAssets.Asset.Id));
            }

            if (!orderFilter.OrderStatuses.IsNullOrEmpty())
            {
                orders = orders.Where(x => orderFilter.OrderStatuses.Contains((OrderStatus)x.OrderStatusID));
            }

            if (orderFilter.CreatedDateStart.HasValue)
            {
                orders = orders.Where(x => x.DateCreated >= orderFilter.CreatedDateStart.Value);
            }

            if (orderFilter.CreatedDateEnd.HasValue)
            {
                orders = orders.Where(x => x.DateCreated <= orderFilter.CreatedDateEnd.Value);
            }

            if (orderFilter.SubmittedDateStart.HasValue)
            {
                orders = orders.Where(x => x.DateCreated >= orderFilter.SubmittedDateStart.Value);
            }

            if (orderFilter.SubmittedDateEnd.HasValue)
            {
                orders = orders.Where(x => x.DateCreated <= orderFilter.SubmittedDateEnd.Value);
            }

            return await orders.ToListAsync();
        }

        #region Mappings

        private void MapOptionsToOrderAsset(OrderDTO orderDTO)
        {
            if (orderDTO?.AssetDTO?.CustomerAssetID <= 0)
            {
                DynamicLogger.LogError("Incorrect CustomerAssetID provided");
                throw new Exception(APITranslations.IncorrectCustomerAssetID);
            }

            var assetOptions = _assetService.GetAssetOptions(orderDTO.AssetDTO.CustomerAssetID);

            orderDTO.AssetDTO.Options = assetOptions
                .Select(x => _mapper.Map<OptionDTO>(x))
                .ToList();
        }

        private async Task<Order> MapDtoToOrderAsync(OrderDTO orderDTO, bool withOptions = false)
        {
            if (withOptions)
            {
                MapOptionsToOrderAsset(orderDTO);
            }

            var order = _mapper.Map<Order>(orderDTO);

            if (!orderDTO.CreatedByUsername.IsNullOrEmpty())
            {
                order.CreatedByUser = await _userManager.FindByNameAsync(orderDTO.CreatedByUsername);
                order.CreatedByUserID = order.CreatedByUser?.Id;
            }

            if (order.CustomerAssets.AssetAddressID.HasValue)
            {
                var assetAddressDTO = orderDTO.CustomerDTO.Addresses.FirstOrDefault(x => x.Id == order.CustomerAssets.AssetAddressID.Value);
                var assetAddress = assetAddressDTO.IsNullOrEmpty()
                    ? await _repository.Addresses.Where(x => x.Id == order.CustomerAssets.AssetAddressID.Value).FirstOrDefaultAsync()
                    : _mapper.Map<Address>(assetAddressDTO);

                order.CustomerAssets.AssetAddress = assetAddress;
            }

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
                DateSubmitted = order.DateSubmitted,
                DateCreated = order.DateCreated
            };

            if (!order.CreatedByUser.IsNullOrEmpty())
            {
                orderDTO.CreatedByUsername = order.CreatedByUser.UserName;
            }

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
            customerAsset.AssetAddressID = order.CustomerAssets.AssetAddressID;
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
