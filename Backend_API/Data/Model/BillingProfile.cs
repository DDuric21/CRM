using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
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

        // fore some to me unkonown reason if the "Foreign key" anotation is added EF tries to create 2 columns with same name
        [ForeignKey("AddressId")]
        public long? AddressID { get; set; }
        public virtual Address Address{ get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public void GenerateKey(int prefix, long billingId)
        {
            BillingProfileId = $"{prefix}-{billingId}";
        }
    }
}
