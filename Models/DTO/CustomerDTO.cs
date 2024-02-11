namespace Models.DTO
{
    public class CustomerDTO
    {
        public string? Name { get; set; }

        public AddressDTO? Address { get; set; }

        public DateTime Birthday { get; set; }

        public ICollection<AssetDTO> Assets { get; set; }
    }
}
