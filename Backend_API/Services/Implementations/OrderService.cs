using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.DTO;

namespace Backend_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICrmRepository _repository;

        public OrderService(ICrmRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateOrder(CustomerAssets customerAsset)
        {
            await _repository.CustomerAssets.InsertAsync(customerAsset);
        }

        public CustomerAssets MapToOrderData(OrderDTO orderDTO)
        {
            var customerAsset = new CustomerAssets();

            customerAsset.AssetID = orderDTO.AssetDTO.Id;
            customerAsset.CustomerID = orderDTO.CustomerDTO.Id;

            return customerAsset;
        }
    }
}
