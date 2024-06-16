using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Asset
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public ICollection<Option>? Options { get; set; }
    }
}
