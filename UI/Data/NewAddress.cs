using Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace UI.Data
{
    public class NewAddress
    {
        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages), 
           ErrorMessageResourceName = nameof(ValidationMessages.AddressFieldRequired))]
        public string Address { get; set; } = string.Empty;

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages), 
           ErrorMessageResourceName = nameof(ValidationMessages.CityFieldRequired))]
        public string City { get; set; } = string.Empty;

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages), 
           ErrorMessageResourceName = nameof(ValidationMessages.PostalCodeFieldRequired))]
        public int PostalCode { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages), 
           ErrorMessageResourceName = nameof(ValidationMessages.CountryFieldRequired))]
        public string Country { get; set; } = string.Empty;
    }
}
