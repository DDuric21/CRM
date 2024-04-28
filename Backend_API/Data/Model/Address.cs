using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Address
    {
        [Key]
        public long Id { get; set; }

        public bool IsLegal { get; set; }

        public string? FullAddress { get; set; }

        [ForeignKey("CustomerId")]
        public long CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
