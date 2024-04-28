namespace Models.DTO
{
    public class AddressDTO
    {
        public long Id { get; set; }

        public bool IsLegal { get; set; }

        public string? FullAddress { get; set; }

        public long? CustomerId { get; set; }
    }
}
