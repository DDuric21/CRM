using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class CustomerAssets : BaseModel
    {
        [ForeignKey("CustomerId")]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(AssetAddressID))]
        public long? AssetAddressID{ get; set; }
        public virtual Address AssetAddress { get; set; }

        [ForeignKey("AssetID")]
        public long AssetID { get; set; }
        public virtual Asset Asset { get; set; }

        public int AssetStatusID { get; set; }

        public string BillingProfileId { get; set; }
        public virtual BillingProfile BillingProfile { get; set; }

        public virtual ICollection<CustomerAssetOptions> CustomerAssetOptions { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
