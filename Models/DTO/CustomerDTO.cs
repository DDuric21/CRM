namespace Models.DTO
{
    public class CustomerDTO
    {
        // should be removed later
        public long Id { get; set; }
        public string? Name { get; set; }

        public AddressDTO? Address { get; set; }

        public DateTime Birthday { get; set; }

        public ICollection<AssetDTO> Assets { get; set; }

        public CustomerDTO()
        {
            Address = new AddressDTO();
        }
    }
}
