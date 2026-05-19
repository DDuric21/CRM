using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class BillingProfile : ITrackChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^\d+-\d+$", ErrorMessage = "The key must be in the format 'number-number' (e.g., '1-45679').")]
        public string BillingProfileId { get; private set; }

        [ForeignKey("CustomerId")]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("AddressId")]
        public long? AddressID { get; set; }
        public virtual Address Address { get; set; }

        public bool Paperless { get; set; }

        public int BillingProfileStatusID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public virtual ICollection<CustomerAssets> CustomerAssets { get; set; }

        public BillingProfile()
        {

        }

        public BillingProfile(string billingProfileId)
        {
            BillingProfileId = billingProfileId;
        }

        public void GenerateKey(int prefix, long billingId)
        {
            BillingProfileId = $"{prefix}-{billingId}";
        }
    }
}
