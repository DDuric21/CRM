using Models.DTO;

namespace Models.Requests
{
    public class CreateOrderRQ
    {
        public OrderDTO OrderDTO { get; set; }

        public bool WithOptions { get; set; }
    }
}
