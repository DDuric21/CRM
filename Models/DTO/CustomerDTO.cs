namespace Models.DTO
{
    public class CustomerDTO
    {
        // should be removed later
        public long Id { get; set; }

        public string? Name { get; set; }

        public List<AddressDTO> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        public List<AssetDTO> Assets { get; set; }

        public CustomerDTO()
        {
            Addresses = new List<AddressDTO>();
            Assets = new List<AssetDTO>();
        }
    }
}
