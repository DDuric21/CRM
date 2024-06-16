using Models.DTO;

namespace UI.Services
{
    public interface IOrderService
    {
        Task AddAsset(OrderDTO orderDTO);
    }
}
