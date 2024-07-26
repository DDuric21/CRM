using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class CustomerAssets
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("CustomerId")]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        public long AssetAddressID{ get; set; }

        [ForeignKey("AssetID")]
        public long AssetID { get; set; }
        public virtual Asset Asset { get; set; }

        public int AssetStatusID { get; set; }

        public virtual ICollection<CustomerAssetOptions> CustomerAssetOptions { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
