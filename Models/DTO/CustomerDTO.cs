namespace Models.DTO
{
    public class CustomerDTO
    {
        // should be removed later
        public long Id { get; set; }

        public string? Name { get; set; }

        public long? LegalAddressId { get; set; }

        public ICollection<AddressDTO> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        public ICollection<AssetDTO> Assets { get; set; }

        public CustomerDTO()
        {
            Addresses = new List<AddressDTO>();
        }
    }
}
