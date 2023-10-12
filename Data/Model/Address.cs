using System.ComponentModel.DataAnnotations;

namespace Backend_API.Data.Model
{
    public class Address
    {
        [Key]
        public long Id { get; set; }

        public string? FullAddress { get; set; }

    }
}
