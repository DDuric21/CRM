using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Address : BaseModel
    {
        public bool IsLegal { get; set; }

        public string? FullAddress { get; set; }

        [ForeignKey("CustomerId")]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual BillingProfile BillingProfile { get; set; }
    }
}
