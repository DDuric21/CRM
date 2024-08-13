using Microsoft.EntityFrameworkCore;

namespace Backend_API.Data.Model
{
    public class Asset : BaseModel
    {
        public string Name { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public virtual ICollection<Option>? Options { get; set; }
    }
}
