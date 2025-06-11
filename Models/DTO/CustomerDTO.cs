using Models.Enums;

namespace Models.DTO
{
    public class CustomerDTO
    {
        // should be removed later
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string PersonalID { get; set; }

        public List<AddressDTO> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        public CustomerType Type { get; set; }

        public ItemState CustomerStatus { get; set; }

        public List<AssetDTO> Assets { get; set; }

        public List<BillingProfileDTO> BillingProfiles { get; set; }

        public CustomerDTO()
        {
            Addresses = new List<AddressDTO>();
            Assets = new List<AssetDTO>();
            BillingProfiles = new List<BillingProfileDTO>();
        }
    }
}
