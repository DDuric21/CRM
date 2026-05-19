using Models.Enums;
using Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class CustomerDTO
    {
        // should be removed later
        public long Id { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.FirstNameFieldRequired))]
        public string? FirstName { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.LastNameFieldRequired))]
        public string? LastName { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.PersonalIDFieldRequired))]
        public string PersonalID { get; set; }

        public List<AddressDTO> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        public CustomerType Type { get; set; }

        public ItemState CustomerStatus { get; set; }

        public List<AssetDTO> Assets { get; set; }

        public List<BillingProfileDTO> BillingProfiles { get; set; }

        public CustomerContactDetails ContactDetails { get; set; }

        public CustomerDTO()
        {
            Addresses = new List<AddressDTO>();
            Assets = new List<AssetDTO>();
            BillingProfiles = new List<BillingProfileDTO>();
        }
    }
}
