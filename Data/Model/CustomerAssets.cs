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

        [ForeignKey("AssetID")]
        public long AssetID { get; set; }
    }
}
