using Models.DTO;

namespace Models.Responses
{
    public class GetOrdersRS : ResponseBase
    {
        public IEnumerable<OrderDTO> Orders { get; set; }
    }
}
