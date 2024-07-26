using Models.Enums;

namespace Models.DTO
{
    public class OrderDTO
    {
        public Guid OrderID { get; set; }

        public CustomerDTO CustomerDTO { get; set; }

        public AssetDTO AssetDTO { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public CrudAction Action { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateSubmited { get; set; }

        public OrderDTO()
        {
            AssetDTO = new AssetDTO();
        }
    }
}
