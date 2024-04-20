using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class CustomerAddresses
    {
        [Key]
        public long Id { get; set; }

        public bool IsLegal { get; set; }

        [ForeignKey("CustomerId")]
        public long CustomerID { get; set; }

        [ForeignKey("AddressID")]
        public long AddressID { get; set; }
    }
}
