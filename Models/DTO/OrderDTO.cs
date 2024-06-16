namespace Models.DTO
{
    public class OrderDTO
    {
        public Guid OrderID { get; set; }

        public CustomerDTO CustomerDTO { get; set; }

        public AssetDTO AssetDTO { get; set; }

        public OrderDTO()
        {
            AssetDTO = new AssetDTO();
        }
    }
}
