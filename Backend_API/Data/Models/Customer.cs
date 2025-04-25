using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class Customer : BaseModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string PersonalID { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        public int TypeID { get; set; }

        public int CustomerStatusID { get; set; }

        [NotMapped]
        public virtual ICollection<Asset> Assets { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<BillingProfile> BillingProfiles { get; set; }
    }
}
