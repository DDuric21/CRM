using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class ContactDetails : BaseModel
    {
        public string? Email1 { get; set; }

        public string? Email2 { get; set; }

        public string? PhoneNumber1 { get; set; }

        public string? PhoneNumber2 { get; set; }

        public string? PhoneNumber3 { get; set; }

        public string? PhoneNumber4 { get; set; }

        public string? FaxNumber1 { get; set; }

        public string? FaxNumber2 { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
