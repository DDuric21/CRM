using System.ComponentModel.DataAnnotations;

namespace Backend_API.Data.Model
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }
        public DateTime Birthday { get; set; }
    }
}
