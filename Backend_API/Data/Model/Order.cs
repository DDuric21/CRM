using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; }


        [ForeignKey("CustomerAssetsID")]
        public long? CustomerAssetsID { get; set; }
        public CustomerAssets CustomerAssets { get; set; }

        public string? Parameters { get; set; }
    }
}
