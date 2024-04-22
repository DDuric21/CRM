using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Address
    {
        [Key]
        public long Id { get; set; }

        public string? FullAddress { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
