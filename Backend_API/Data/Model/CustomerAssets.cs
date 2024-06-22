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
        public Customer Customer { get; set; }

        public long AssetAddressID{ get; set; }

        [ForeignKey("AssetID")]
        public long AssetID { get; set; }
        public Asset Asset { get; set; }

        public ICollection<CustomerAssetOptions> CustomerAssetOptions { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
