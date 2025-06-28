using Models.Enums;

namespace Models.DTO
{
    public class BillingProfileDTO
    {
        public string BillingProfileId { get; set; }

        public long CustomerID { get; set; }

        public bool Paperless { get; set; }

        public ItemState BillingProfileStatus { get; set; }

        public AddressDTO BillingAddress { get; set; } = new AddressDTO();
    }
}
