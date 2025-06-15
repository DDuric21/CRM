using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class Address : BaseModel
    {
        public bool IsLegal { get; set; }

        public string? FullAddress { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<BillingProfile> BillingProfiles { get; set; }

        public virtual ICollection<CustomerAssets> CustomerAssets { get; set; }
    }
}
