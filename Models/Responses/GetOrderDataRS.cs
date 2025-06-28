using Models.DTO;

namespace Models.Responses
{
    public class GetOrderDataRS : ResponseBase
    {
        public OrderDTO Order { get; set; } = new OrderDTO();
    }
}
