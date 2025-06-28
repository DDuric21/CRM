using Models.DTO;

namespace Models.Requests
{
    public class CreateOrderRQ : RequestBase
    {
        public OrderDTO OrderDTO { get; set; }

        public bool WithOptions { get; set; }
    }
}
