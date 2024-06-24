using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; }

        [ForeignKey("CustomerID")]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("CustomerAssetsID")]
        public Nullable<long> CustomerAssetsID { get; set; }
        [NotMapped]
        public virtual CustomerAssets CustomerAssets { get; set; }

        public string? Parameters { get; set; }
    }
}
