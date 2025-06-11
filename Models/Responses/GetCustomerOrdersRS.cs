using Models.DTO;

namespace Models.Responses
{
    public class GetCustomerOrdersRS : ResponseBase
    {
        public IEnumerable<OrderDTO> Orders { get; set; }
    }
}
