using Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class CustomerContactDetails
    {
        [RegularExpression(
            @"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessageResourceType = typeof(ValidationMessages),
            ErrorMessageResourceName = nameof(ValidationMessages.EmailNotInValidFormat))]
        public string? Email1 { get; set; }

        [RegularExpression(
            @"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessageResourceType = typeof(ValidationMessages),
            ErrorMessageResourceName = nameof(ValidationMessages.EmailNotInValidFormat))]
        public string? Email2 { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.MainPhoneNumberIsRequired))]
        public string? PhoneNumber1 { get; set; }

        public string? PhoneNumber2 { get; set; }

        public string? PhoneNumber3 { get; set; }

        public string? PhoneNumber4 { get; set; }

        public string? FaxNumber1 { get; set; }

        public string? FaxNumber2 { get; set; }
    }
}
