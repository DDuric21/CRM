using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }

        public string? Name { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public DateTime Birthday { get; set; }

        [NotMapped]
        public virtual ICollection<Asset> Assets { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
