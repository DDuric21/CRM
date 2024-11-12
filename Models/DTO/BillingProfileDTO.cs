namespace Models.DTO
{
    public class BillingProfileDTO
    {
        public string BillingProfileId { get; set; }

        public long CustomerID { get; set; }

        public AddressDTO BillingAddress { get; set; }
    }
}
