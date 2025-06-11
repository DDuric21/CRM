using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Models
{
    public class Order : ITrackChanges
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

        public int OrderStatusID { get; set; }

        public int ActionID { get; set; }

        [ForeignKey(nameof(CreatedByUserID))]
        public string CreatedByUserID { get; set; }

        public virtual User CreatedByUser { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime DateSubmitted { get; set; }

        public string? Parameters { get; set; }
    }
}
